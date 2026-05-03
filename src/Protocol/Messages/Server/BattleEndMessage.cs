using CRServer.Database;
using CRServer.Network;

namespace CRServer.Protocol.Messages.Server;

public class BattleEndMessage : PiranhaMessage
{
    private readonly bool _isVictory;
    private readonly int _myCrowns;
    private readonly int _enemyCrowns;
    private readonly int _trophyChange;

    public BattleEndMessage(
        Session session,
        DatabaseManager database,
        bool isVictory,
        int myCrowns,
        int enemyCrowns,
        int trophyChange)
        : base(20116, session, new ByteStream(Array.Empty<byte>()), database)
    {
        _isVictory = isVictory;
        _myCrowns = myCrowns;
        _enemyCrowns = enemyCrowns;
        _trophyChange = trophyChange;
    }

    public override void Decode() { }
    public override Task ProcessAsync() => Task.CompletedTask;

    protected override void Encode(ByteWriter writer)
    {
        // Resultado da batalha
        writer.WriteBool(_isVictory);       // Ganhou?
        writer.WriteInt(_myCrowns);          // Minhas coroas
        writer.WriteInt(_enemyCrowns);       // Coroas do inimigo
        writer.WriteInt(_trophyChange);      // Troféus ganhos/perdidos (positivo ou negativo)
        writer.WriteInt(0);                  // Ouro ganho
        writer.WriteInt(0);                  // Experiência ganha
    }
}
