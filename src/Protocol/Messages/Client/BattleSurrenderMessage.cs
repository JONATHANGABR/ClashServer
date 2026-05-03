using CRServer.Database;
using CRServer.Logic.Battle;
using CRServer.Network;
using Serilog;

namespace CRServer.Protocol.Messages.Client;

public class BattleSurrenderMessage : PiranhaMessage
{
    private readonly BattleManager _battleManager;

    public BattleSurrenderMessage(Session session, ByteStream stream, DatabaseManager database, BattleManager battleManager)
        : base(14102, session, stream, database)
    {
        _battleManager = battleManager;
    }

    public override void Decode() { }

    public override async Task ProcessAsync()
    {
        if (Session.Player == null) return;

        var battle = _battleManager.GetBattleByPlayer(Session.AccountId);
        if (battle == null)
        {
            Log.Warning($"[BATTLE] {Session.Player.Name} tentou desistir mas não está em batalha");
            return;
        }

        var battlePlayer = battle.Player1.AccountId == Session.AccountId
            ? battle.Player1
            : battle.Player2;

        Log.Information($"[BATTLE] {Session.Player.Name} desistiu da batalha!");
        await battle.SurrenderAsync(battlePlayer);
    }
}
