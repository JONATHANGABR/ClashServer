using CRServer.Database;
using CRServer.Network;

namespace CRServer.Protocol.Messages.Server;

public class SectorStateMessage : PiranhaMessage
{
    private readonly int _waitingSeconds;
    private readonly int _queueCount;

    public SectorStateMessage(Session session, DatabaseManager database, int waitingSeconds, int queueCount)
        : base(20107, session, new ByteStream(Array.Empty<byte>()), database)
    {
        _waitingSeconds = waitingSeconds;
        _queueCount = queueCount;
    }

    public override void Decode() { }
    public override Task ProcessAsync() => Task.CompletedTask;

    protected override void Encode(ByteWriter writer)
    {
        writer.WriteInt(_waitingSeconds);   // Segundos esperando
        writer.WriteInt(_queueCount);       // Jogadores na fila
        writer.WriteInt(60);                // Tempo máximo antes do bot
    }
}
