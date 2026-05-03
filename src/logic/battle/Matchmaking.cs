using CRServer.Database;
using CRServer.Logic.Player;
using CRServer.Network;
using Serilog;

namespace CRServer.Logic.Battle;

public class MatchmakingEntry
{
    public BattlePlayer BattlePlayer { get; set; }
    public DateTime EnqueuedAt { get; set; }
    public Session Session { get; set; }

    public MatchmakingEntry(BattlePlayer battlePlayer, Session session)
    {
        BattlePlayer = battlePlayer;
        Session = session;
        EnqueuedAt = DateTime.UtcNow;
    }

    public double SecondsWaiting => (DateTime.UtcNow - EnqueuedAt).TotalSeconds;
}

public class Matchmaking
{
    // Tempo máximo para encontrar partida antes de dar bot (60 segundos)
    private const int MaxWaitSeconds = 60;

    private readonly List<MatchmakingEntry> _queue = new();
    private readonly BattleManager _battleManager;
    private readonly DatabaseManager _database;
    private readonly CancellationTokenSource _cts = new();

    public int QueueCount => _queue.Count;

    public Matchmaking(BattleManager battleManager, DatabaseManager database)
    {
        _battleManager = battleManager;
        _database = database;
    }

    public void Start()
    {
        _ = Task.Run(() => MatchmakingLoopAsync(_cts.Token));
        Log.Information("[MATCHMAKING] Sistema iniciado!");
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    public bool EnqueuePlayer(BattlePlayer player, Session session)
    {
        lock (_queue)
        {
            // Verificar se já está na fila
            if (_queue.Any(e => e.BattlePlayer.AccountId == player.AccountId))
            {
                Log.Warning($"[MATCHMAKING] {player.Name} já está na fila!");
                return false;
            }

            _queue.Add(new MatchmakingEntry(player, session));
            Log.Information($"[MATCHMAKING] {player.Name} entrou na fila! ({_queue.Count} na fila)");
            return true;
        }
    }

    public void DequeuePlayer(long accountId)
    {
        lock (_queue)
        {
            var entry = _queue.FirstOrDefault(e => e.BattlePlayer.AccountId == accountId);
            if (entry != null)
            {
                _queue.Remove(entry);
                Log.Information($"[MATCHMAKING] Jogador {accountId} saiu da fila");
            }
        }
    }

    private async Task MatchmakingLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(2000, cancellationToken); // Verificar a cada 2s
                await ProcessQueueAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[MATCHMAKING] Erro no loop de matchmaking");
            }
        }
    }

    private async Task ProcessQueueAsync()
    {
        List<MatchmakingEntry> toProcess;

        lock (_queue)
        {
            toProcess = _queue.ToList();
        }

        if (toProcess.Count == 0) return;

        var matched = new HashSet<long>();

        // Tentar fazer match entre jogadores
        for (int i = 0; i < toProcess.Count; i++)
        {
            var entry1 = toProcess[i];
            if (matched.Contains(entry1.BattlePlayer.AccountId)) continue;

            bool foundMatch = false;

            for (int j = i + 1; j < toProcess.Count; j++)
            {
                var entry2 = toProcess[j];
                if (matched.Contains(entry2.BattlePlayer.AccountId)) continue;

                // Verificar se troféus são compatíveis
                if (IsTrophyCompatible(entry1.BattlePlayer, entry2.BattlePlayer))
                {
                    // ✅ Match PvP encontrado!
                    matched.Add(entry1.BattlePlayer.AccountId);
                    matched.Add(entry2.BattlePlayer.AccountId);

                    Log.Information($"[MATCHMAKING] ✅ PvP Match! {entry1.BattlePlayer.Name} vs {entry2.BattlePlayer.Name}");

                    RemoveFromQueue(entry1.BattlePlayer.AccountId);
                    RemoveFromQueue(entry2.BattlePlayer.AccountId);

                    await _battleManager.StartPvPBattleAsync(entry1, entry2);
                    foundMatch = true;
                    break;
                }
            }

            // Sem match e esperando mais de 60 segundos → PvE com BOT
            if (!foundMatch && entry1.SecondsWaiting >= MaxWaitSeconds)
            {
                matched.Add(entry1.BattlePlayer.AccountId);

                Log.Information($"[MATCHMAKING] ⏰ Tempo esgotado! {entry1.BattlePlayer.Name} vai jogar contra BOT");

                RemoveFromQueue(entry1.BattlePlayer.AccountId);
                await _battleManager.StartPvEBattleAsync(entry1);
            }
        }
    }

    private bool IsTrophyCompatible(BattlePlayer p1, BattlePlayer p2)
    {
        // Tolerância de troféus aumenta com o tempo de espera
        int baseTolerance = 300;
        int tolerance = baseTolerance;

        // Calcula o maior tempo de espera entre os dois
        double maxWait = Math.Max(
            (DateTime.UtcNow - DateTime.UtcNow).TotalSeconds,
            0);

        // A cada 10 segundos esperando, aumenta tolerância em 100 troféus
        tolerance += (int)(maxWait / 10) * 100;

        return Math.Abs(p1.Trophies - p2.Trophies) <= tolerance;
    }

    private void RemoveFromQueue(long accountId)
    {
        lock (_queue)
        {
            var entry = _queue.FirstOrDefault(e => e.BattlePlayer.AccountId == accountId);
            if (entry != null) _queue.Remove(entry);
        }
    }
}
