using CRServer.Database;
using CRServer.Logic.Battle;
using CRServer.Network;
using Serilog;

namespace CRServer.Protocol.Messages.Client;

public class GoHomeMessage : PiranhaMessage
{
    private readonly BattleManager _battleManager;

    public GoHomeMessage(Session session, ByteStream stream, DatabaseManager database, BattleManager battleManager)
        : base(14113, session, stream, database)
    {
        _battleManager = battleManager;
    }

    public override void Decode() { }

    public override async Task ProcessAsync()
    {
        if (Session.Player == null) return;

        Log.Information($"[HOME] {Session.Player.Name} voltou para home");

        // Remover da fila de matchmaking se estiver
        _battleManager.GetMatchmaking().DequeuePlayer(Session.AccountId);

        // Verificar se está em batalha e desistir
        var battle = _battleManager.GetBattleByPlayer(Session.AccountId);
        if (battle != null && !battle.IsFinished)
        {
            var battlePlayer = battle.Player1.AccountId == Session.AccountId
                ? battle.Player1
                : battle.Player2;

            await battle.SurrenderAsync(battlePlayer);
        }
    }
}
