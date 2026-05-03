using CRServer.Config;
using CRServer.Database;
using CRServer.Logic.Battle;
using CRServer.Network;
using CRServer.Protocol.Messages.Client;

namespace CRServer.Protocol;

public static class MessageFactory
{
    public static PiranhaMessage? Create(
        ushort id,
        Session session,
        ByteStream stream,
        DatabaseManager database,
        GameSettings gameSettings,
        ServerSettings serverSettings,
        BattleManager battleManager)
    {
        return id switch
        {
            // ─── Cliente → Servidor ───────────────────────────────
            10100 => new LoginMessage(session, stream, database, gameSettings, serverSettings),
            10108 => new KeepAliveMessage(session, stream, database),
            14101 => new BattleStartMessage(session, stream, database, battleManager),
            14102 => new BattleSurrenderMessage(session, stream, database, battleManager),
            14113 => new GoHomeMessage(session, stream, database, battleManager),
            // ─────────────────────────────────────────────────────

            _ => null
        };
    }
}
