using CRServer.Config;
using CRServer.Database;
using CRServer.Logic.Battle;
using CRServer.Network;
using Serilog;

namespace CRServer.Core;

public class ServerCore
{
    private readonly ServerConfig _config;
    private readonly DatabaseManager _database;
    private readonly BattleManager _battleManager;
    private readonly Listener _listener;

    public ServerCore(ServerConfig config)
    {
        _config = config;
        _database = new DatabaseManager(config.Database);
        _battleManager = new BattleManager(_database);
        _listener = new Listener(config.Server, _database, config.Game, _battleManager);
    }

    public async Task StartAsync()
    {
        Log.Information("=========================================");
        Log.Information("  Clash Royale Private Server v3.2803.3");
        Log.Information("=========================================");

        // Banco de dados
        Log.Information("Conectando ao MySQL...");
        await _database.ConnectAsync();
        Log.Information("✓ MySQL conectado!");

        // Battle Manager
        _battleManager.Start();
        Log.Information("✓ Sistema de batalhas iniciado!");

        // Listener TCP
        Log.Information($"Iniciando servidor na porta {_config.Server.Port}...");
        await _listener.StartAsync();
    }

    public async Task StopAsync()
    {
        Log.Information("Desligando servidor...");
        _battleManager.Stop();
        await _listener.StopAsync();
        await _database.DisconnectAsync();
        Log.Information("✓ Servidor desligado!");
    }
}
