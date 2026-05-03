using System.Net.Sockets;
using CRServer.Config;
using CRServer.Database;
using CRServer.Logic.Battle;
using CRServer.Logic.Player;
using CRServer.Protocol;
using Serilog;

namespace CRServer.Network;

public class Session
{
    private readonly TcpClient _client;
    private readonly Listener _listener;
    private readonly DatabaseManager _database;
    private readonly GameSettings _gameSettings;
    private readonly ServerSettings _serverSettings;
    private readonly BattleManager _battleManager;
    private readonly NetworkStream _stream;
    private bool _connected = true;

    public long AccountId { get; set; }
    public PlayerData? Player { get; set; }
    public string RemoteAddress { get; }

    public Session(
        TcpClient client,
        Listener listener,
        DatabaseManager database,
        GameSettings gameSettings,
        ServerSettings serverSettings,
        BattleManager battleManager)
    {
        _client = client;
        _listener = listener;
        _database = database;
        _gameSettings = gameSettings;
        _serverSettings = serverSettings;
        _battleManager = battleManager;
        _stream = client.GetStream();
        RemoteAddress = client.Client.RemoteEndPoint?.ToString() ?? "?";
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var headerBuffer = new byte[7];

        while (_connected && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                int headerRead = await ReadExactAsync(headerBuffer, 7, cancellationToken);
                if (headerRead == 0) break;

                ushort messageId = (ushort)((headerBuffer[0] << 8) | headerBuffer[1]);
                int payloadLen = (headerBuffer[2] << 16) | (headerBuffer[3] << 8) | headerBuffer[4];
                ushort version = (ushort)((headerBuffer[5] << 8) | headerBuffer[6]);

                byte[] payload = new byte[payloadLen];
                if (payloadLen > 0)
                {
                    int payloadRead = await ReadExactAsync(payload, payloadLen, cancellationToken);
                    if (payloadRead == 0) break;
                }

                Log.Debug($"[RECV] [{RemoteAddress}] ID:{messageId} Len:{payloadLen}");

                var stream = new ByteStream(payload);
                var message = MessageFactory.Create(
                    messageId, this, stream,
                    _database, _gameSettings,
                    _serverSettings, _battleManager);

                if (message != null)
                {
                    message.Decode();
                    await message.ProcessAsync();
                }
                else
                {
                    Log.Warning($"[WARN] Mensagem desconhecida: {messageId}");
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                Log.Error(ex, $"Erro na sessão {RemoteAddress}");
                break;
            }
        }

        await DisconnectAsync("Sessão encerrada");
    }

    public async Task SendAsync(PiranhaMessage message)
    {
        try
        {
            var data = message.Build();
            await _stream.WriteAsync(data);
            Log.Debug($"[SEND] [{RemoteAddress}] ID:{message.MessageId}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Erro ao enviar para {RemoteAddress}");
        }
    }

    public async Task DisconnectAsync(string reason = "")
    {
        if (!_connected) return;
        _connected = false;

        // Remover da fila de matchmaking se necessário
        _battleManager.GetMatchmaking().DequeuePlayer(AccountId);

        if (!string.IsNullOrEmpty(reason))
            Log.Information($"Desconectado [{RemoteAddress}]: {reason}");

        try { _stream.Close(); _client.Close(); } catch { }

        _listener.RemoveSession(this);
    }

    private async Task<int> ReadExactAsync(byte[] buffer, int count, CancellationToken ct)
    {
        int totalRead = 0;
        while (totalRead < count)
        {
            int read = await _stream.ReadAsync(buffer.AsMemory(totalRead, count - totalRead), ct);
            if (read == 0) return 0;
            totalRead += read;
        }
        return totalRead;
    }
}
