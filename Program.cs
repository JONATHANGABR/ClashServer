using CRServer.Config;
using CRServer.Core;
using Serilog;

var cfg = ServerConfig.Load("serverconfig.json");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/server-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Boot do servidor...");
    var core = new ServerCore(cfg);

    Console.CancelKeyPress += async (_, e) =>
    {
        e.Cancel = true;
        await core.StopAsync();
    };

    await core.StartAsync(); // bloqueia aqui até StopAsync() disparar
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal");
}
finally
{
    Log.CloseAndFlush();
}