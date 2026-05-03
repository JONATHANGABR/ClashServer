using CRServer.Database;
using CRServer.Logic.Battle;
using CRServer.Network;
using Serilog;

namespace CRServer.Protocol.Messages.Client;

public class BattleStartMessage : PiranhaMessage
{
    private readonly BattleManager _battleManager;

    public BattleStartMessage(Session session, ByteStream stream, DatabaseManager database, BattleManager battleManager)
        : base(14101, session, stream, database)
    {
        _battleManager = battleManager;
    }

    public override void Decode() { }

    public override async Task ProcessAsync()
    {
        if (Session.Player == null)
        {
            Log.Warning($"[BATTLE] Sessão sem jogador tentou iniciar batalha");
            return;
        }

        // Verificar se já está em batalha
        var existingBattle = _battleManager.GetBattleByPlayer(Session.AccountId);
        if (existingBattle != null)
        {
            Log.Warning($"[BATTLE] {Session.Player.Name} já está em uma batalha!");
            return;
        }

        Log.Information($"[BATTLE] {Session.Player.Name} entrou na fila de matchmaking!");

        // Criar BattlePlayer
        var battlePlayer = BattlePlayer.FromPlayer(Session.Player, Session);

        // Entrar na fila de matchmaking
        _battleManager.GetMatchmaking().EnqueuePlayer(battlePlayer, Session);
    }
}
