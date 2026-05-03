using CRServer.Database;
using Serilog;

namespace CRServer.Logic.Battle;

public class BattleManager
{
    private readonly Dictionary<Guid, BattleRoom> _activeBattles = new();
    private readonly DatabaseManager _database;
    private readonly Matchmaking _matchmaking;

    public int ActiveBattleCount => _activeBattles.Count;

    public BattleManager(DatabaseManager database)
    {
        _database = database;
        _matchmaking = new Matchmaking(this, database);
    }

    public void Start()
    {
        _matchmaking.Start();
        Log.Information("[BATTLE] BattleManager iniciado!");
    }

    public void Stop()
    {
        _matchmaking.Stop();
    }

    public Matchmaking GetMatchmaking() => _matchmaking;

    // Iniciar batalha PvP
    public async Task StartPvPBattleAsync(MatchmakingEntry entry1, MatchmakingEntry entry2)
    {
        var room = new BattleRoom(
            entry1.BattlePlayer,
            entry2.BattlePlayer,
            BattleType.PvP,
            _database);

        lock (_activeBattles)
        {
            _activeBattles[room.RoomId] = room;
        }

        await room.StartAsync();

        // Remover batalha finalizada após 10s
        _ = Task.Delay(10000).ContinueWith(_ => RemoveBattle(room.RoomId));
    }

    // Iniciar batalha PvE (contra bot)
    public async Task StartPvEBattleAsync(MatchmakingEntry entry)
    {
        var bot = BattlePlayer.CreateBot(entry.BattlePlayer.Trophies);

        var room = new BattleRoom(
            entry.BattlePlayer,
            bot,
            BattleType.PvE,
            _database);

        lock (_activeBattles)
        {
            _activeBattles[room.RoomId] = room;
        }

        await room.StartAsync();

        _ = Task.Delay(10000).ContinueWith(_ => RemoveBattle(room.RoomId));
    }

    public BattleRoom? GetBattleByPlayer(long accountId)
    {
        lock (_activeBattles)
        {
            return _activeBattles.Values.FirstOrDefault(b =>
                b.Player1.AccountId == accountId ||
                b.Player2.AccountId == accountId);
        }
    }

    private void RemoveBattle(Guid roomId)
    {
        lock (_activeBattles)
        {
            if (_activeBattles.Remove(roomId))
                Log.Debug($"[BATTLE] Sala {roomId} removida. Ativas: {_activeBattles.Count}");
        }
    }
}
