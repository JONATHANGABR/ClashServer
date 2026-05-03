using Serilog;

namespace CRServer.Logic.Battle.Bot;

public class BotAI
{
    private readonly BattleRoom _room;
    private readonly BattlePlayer _bot;
    private readonly Random _random = new();
    private CancellationTokenSource? _cts;

    public BotAI(BattleRoom room, BattlePlayer bot)
    {
        _room = room;
        _bot = bot;
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        _ = Task.Run(() => RunAsync(_cts.Token));
    }

    public void Stop()
    {
        _cts?.Cancel();
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        Log.Debug($"[BOT] {_bot.Name} iniciou IA na batalha {_room.RoomId}");

        while (!cancellationToken.IsCancellationRequested && !_room.IsFinished)
        {
            try
            {
                // Simula o bot "pensando" e jogando cartas
                await Task.Delay(_random.Next(3000, 8000), cancellationToken);

                if (_room.IsFinished) break;

                // Bot ataca torres aleatoriamente
                int damage = _random.Next(100, 400);
                _room.BotAttack(_bot, damage);

                Log.Debug($"[BOT] {_bot.Name} causou {damage} de dano");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[BOT] Erro na IA do bot {_bot.Name}");
                break;
            }
        }

        Log.Debug($"[BOT] {_bot.Name} encerrou IA");
    }
}
