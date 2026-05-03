using CRServer.Database;
using CRServer.Logic.Battle.Bot;
using CRServer.Protocol.Messages.Server;
using MySqlConnector;
using Serilog;

namespace CRServer.Logic.Battle;

public enum BattleState
{
    WaitingPlayers,
    InProgress,
    Finished
}

public enum BattleType
{
    PvP = 0,
    PvE = 1
}

public class BattleRoom
{
    public Guid RoomId { get; } = Guid.NewGuid();
    public BattleType Type { get; }
    public BattleState State { get; private set; } = BattleState.WaitingPlayers;
    public bool IsFinished => State == BattleState.Finished;

    public BattlePlayer Player1 { get; }
    public BattlePlayer Player2 { get; }

    private readonly DatabaseManager _database;
    private readonly CancellationTokenSource _cts = new();
    private BotAI? _botAI;

    // Timer da batalha (3 minutos = 180 segundos)
    private const int BattleDurationSeconds = 180;

    public BattleRoom(BattlePlayer p1, BattlePlayer p2, BattleType type, DatabaseManager database)
    {
        Player1 = p1;
        Player2 = p2;
        Type = type;
        _database = database;
    }

    public async Task StartAsync()
    {
        State = BattleState.InProgress;

        Log.Information($"[BATTLE] Batalha iniciada! {Player1.Name} vs {Player2.Name} | Tipo: {Type} | ID: {RoomId}");

        // Notificar jogadores que a batalha começou
        await NotifyBattleStartAsync();

        // Se for PvE, iniciar IA do bot
        if (Type == BattleType.PvE)
        {
            _botAI = new BotAI(this, Player2);
            _botAI.Start();
        }

        // Timer da batalha
        _ = Task.Run(() => BattleTimerAsync(_cts.Token));
    }

    private async Task NotifyBattleStartAsync()
    {
        // Notificar Player1
        if (Player1.Session != null)
        {
            await Player1.Session.SendAsync(
                new EnemyFoundMessage(Player1.Session, _database, Player2));
        }

        // Notificar Player2 (se for PvP)
        if (Type == BattleType.PvP && Player2.Session != null)
        {
            await Player2.Session.SendAsync(
                new EnemyFoundMessage(Player2.Session, _database, Player1));
        }
    }

    private async Task BattleTimerAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(BattleDurationSeconds), cancellationToken);

            if (!IsFinished)
            {
                // Tempo esgotado — determinar vencedor por coroas
                Log.Information($"[BATTLE] Tempo esgotado! {RoomId}");
                await EndBattleAsync(DetermineWinnerByTrophies());
            }
        }
        catch (OperationCanceledException) { }
    }

    // Bot ataca torres
    public void BotAttack(BattlePlayer bot, int damage)
    {
        if (IsFinished) return;

        Player1.TowerHp = Math.Max(0, Player1.TowerHp - damage);

        // Simular coroas baseado no HP
        if (Player1.TowerHp <= 0)
        {
            Player2.CrownsTaken = Math.Min(Player2.CrownsTaken + 1, 3);
            Player1.TowerHp = 3000; // Reset torre para próxima

            if (Player2.CrownsTaken >= 3)
            {
                _ = EndBattleAsync(Player2);
            }
        }
    }

    // Jogador ataca torres
    public async Task PlayerAttackAsync(BattlePlayer attacker, int damage)
    {
        if (IsFinished) return;

        var defender = attacker == Player1 ? Player2 : Player1;
        defender.TowerHp = Math.Max(0, defender.TowerHp - damage);

        if (defender.TowerHp <= 0)
        {
            attacker.CrownsTaken = Math.Min(attacker.CrownsTaken + 1, 3);
            defender.TowerHp = 3000;

            Log.Information($"[BATTLE] {attacker.Name} pegou coroa! {attacker.CrownsTaken}/3");

            if (attacker.CrownsTaken >= 3)
            {
                await EndBattleAsync(attacker);
            }
        }
    }

    public async Task SurrenderAsync(BattlePlayer player)
    {
        if (IsFinished) return;

        player.SurrenderedOrLeft = true;
        Log.Information($"[BATTLE] {player.Name} desistiu!");

        var winner = player == Player1 ? Player2 : Player1;
        await EndBattleAsync(winner);
    }

    private BattlePlayer DetermineWinnerByTrophies()
    {
        if (Player1.CrownsTaken > Player2.CrownsTaken)
            return Player1;
        if (Player2.CrownsTaken > Player1.CrownsTaken)
            return Player2;

        // Empate — vence quem tem mais troféus
        return Player1.Trophies >= Player2.Trophies ? Player1 : Player2;
    }

    public async Task EndBattleAsync(BattlePlayer winner)
    {
        if (IsFinished) return;

        State = BattleState.Finished;
        _cts.Cancel();
        _botAI?.Stop();

        var loser = winner == Player1 ? Player2 : Player1;

        // Calcular troféus ganhos/perdidos
        int trophyChange = CalculateTrophyChange(winner, loser);

        Log.Information($"[BATTLE] {winner.Name} venceu! Coroas: {winner.CrownsTaken}x{loser.CrownsTaken} | Troféus: +{trophyChange}");

        // Atualizar troféus no banco
        await UpdateTrophiesAsync(winner, loser, trophyChange);

        // Salvar batalha no banco
        await SaveBattleAsync(winner, loser);

        // Notificar jogadores do resultado
        await NotifyBattleEndAsync(winner, loser, trophyChange);
    }

    private int CalculateTrophyChange(BattlePlayer winner, BattlePlayer loser)
    {
        // Fórmula simples de troféus
        int baseTrophy = 30;
        int diff = winner.Trophies - loser.Trophies;

        if (diff > 300) return Math.Max(5, baseTrophy - 20);
        if (diff < -300) return Math.Min(60, baseTrophy + 20);

        return baseTrophy;
    }

    private async Task UpdateTrophiesAsync(BattlePlayer winner, BattlePlayer loser, int trophyChange)
    {
        try
        {
            var conn = _database.GetConnection();

            // Atualizar vencedor (apenas se for jogador real)
            if (!winner.IsBot)
            {
                var cmdWinner = new MySqlCommand(@"
                    UPDATE players 
                    SET trophies = trophies + @change,
                        best_trophies = GREATEST(best_trophies, trophies + @change)
                    WHERE account_id = @id", conn);
                cmdWinner.Parameters.AddWithValue("@change", trophyChange);
                cmdWinner.Parameters.AddWithValue("@id", winner.AccountId);
                await cmdWinner.ExecuteNonQueryAsync();
            }

            // Atualizar perdedor (apenas se for jogador real)
            if (!loser.IsBot)
            {
                int trophyLoss = Math.Max(0, trophyChange - 5);
                var cmdLoser = new MySqlCommand(@"
                    UPDATE players 
                    SET trophies = GREATEST(0, trophies - @loss)
                    WHERE account_id = @id", conn);
                cmdLoser.Parameters.AddWithValue("@loss", trophyLoss);
                cmdLoser.Parameters.AddWithValue("@id", loser.AccountId);
                await cmdLoser.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BATTLE] Erro ao atualizar troféus");
        }
    }

    private async Task SaveBattleAsync(BattlePlayer winner, BattlePlayer loser)
    {
        try
        {
            var conn = _database.GetConnection();
            var cmd = new MySqlCommand(@"
                INSERT INTO battles
                (player1_id, player2_id, winner_id, crowns_p1, crowns_p2, battle_type)
                VALUES (@p1, @p2, @winner, @c1, @c2, @type)", conn);

            cmd.Parameters.AddWithValue("@p1", Player1.AccountId);
            cmd.Parameters.AddWithValue("@p2", Player2.AccountId);
            cmd.Parameters.AddWithValue("@winner", winner.AccountId);
            cmd.Parameters.AddWithValue("@c1", Player1.CrownsTaken);
            cmd.Parameters.AddWithValue("@c2", Player2.CrownsTaken);
            cmd.Parameters.AddWithValue("@type", (int)Type);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[BATTLE] Erro ao salvar batalha no banco");
        }
    }

    private async Task NotifyBattleEndAsync(BattlePlayer winner, BattlePlayer loser, int trophyChange)
    {
        // Notificar Player1
        if (Player1.Session != null)
        {
            bool p1Won = Player1 == winner;
            await Player1.Session.SendAsync(new BattleEndMessage(
                Player1.Session,
                _database,
                p1Won,
                Player1.CrownsTaken,
                Player2.CrownsTaken,
                p1Won ? trophyChange : -(trophyChange - 5)));
        }

        // Notificar Player2 se PvP
        if (Type == BattleType.PvP && Player2.Session != null)
        {
            bool p2Won = Player2 == winner;
            await Player2.Session.SendAsync(new BattleEndMessage(
                Player2.Session,
                _database,
                p2Won,
                Player2.CrownsTaken,
                Player1.CrownsTaken,
                p2Won ? trophyChange : -(trophyChange - 5)));
        }
    }
}
