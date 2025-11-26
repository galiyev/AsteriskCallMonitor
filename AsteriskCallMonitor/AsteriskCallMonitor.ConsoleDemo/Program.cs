using AsteriskCallMonitor.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Sufficit.Asterisk.Manager;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/asterisk.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 10)
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()  // <-- подключение Serilog
    .ConfigureServices(services =>
    {
        services.Configure<AMIProviderOptions>(opt =>
        {
            opt.Address = "10.10.111.5";
            opt.Username = "112";
            opt.Password = "123456789Aa";
            opt.KeepAlive = true;
        });

        services.AddSingleton<IAsteriskMonitor, AsteriskMonitor>();
    })
    .Build();

var monitor = host.Services.GetRequiredService<IAsteriskMonitor>();

await monitor.StartAsync(CancellationToken.None);

Console.WriteLine("Monitoring incoming SIP calls. Press ENTER to exit...");
Console.ReadLine();

await monitor.StopAsync();