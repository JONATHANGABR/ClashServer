using System.Net;
using System.Net.Sockets;
using CRServer.Config;
using CRServer.Database;
using CRServer.Logic.Battle;
using Serilog;

namespace CRServer.Network;

public class Listener
{
    private readonly ServerSettings _settings;
    private readonly DatabaseManager _database;
    private readonly GameSettings _gameSettings;
    private readonly BattleManager _battleManager;
    private TcpListener? _tcpListener;
    private readonly List<Session> _sessions = new();
    private readonly CancellationTokenSource _cts = new();

    public Listener(
        ServerSettings settings,
        DatabaseManager database,
        GameSettings gameSettings,
        BattleManager battleManager)
    {
        _settings = settings;
        _database = database;
        _gameSettings = gameSettings;
        _battleManager = battleManager;
    }

    public async Task StartAsync()
    {
        _tcpListener = new TcpListener(IPAddress.Parse(_settings.Host), _settings.Port);
        _tcpListener.Start();

        Log.Information($"✓ Servidor em {_settings.Host}:{_settings.Port}");
        Log.Information($"✓ Máximo de jogadores: {_settings.MaxPlayers}");
        Log.Information("Aguardando conexões...\n");

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var client = await _tcpListener.AcceptTcpClientAsync(_cts.Token);
                var remote = client.Client.RemoteEndPoint?.ToString() ?? "?";

                if (_sessions.Count >= _settings.MaxPlayers)
                {
                    Log.Warning($"Servidor cheio! Rejeitando: {remote}");
                    client.Close();
                    continue;
                }

                Log.Information($"Nova conexão: {remote} | Online: {_sessions.Count + 1}");

                var session = new Session(
                    client, this,
                    _database, _gameSettings,
                    _settings, _battleManager);

                lock (_sessions) { _sessions.Add(session); }

                _ = Task.Run(() => session.ProcessAsync(_cts.Token));
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex) { Log.Error(ex, "Erro ao aceitar conexão"); }
        }
    }

    public async Task StopAsync()
    {
        _cts.Cancel();
        _tcpListener?.Stop();
        foreach (var session in _sessions.ToList())
            await session.DisconnectAsync("Servidor desligado");
        _sessions.Clear();
    }

    public void RemoveSession(Session session)
    {
        lock (_sessions) { _sessions.Remove(session); }
        Log.Information($"Sessão encerrada. Online: {_sessions.Count}");
    }
}
