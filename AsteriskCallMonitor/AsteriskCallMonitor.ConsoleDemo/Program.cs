using AsteriskCallMonitor.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sufficit.Asterisk.Manager;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Настройки AMI
        services.Configure<AMIProviderOptions>(opt =>
        {
            opt.Address = "10.10.111.5";
            opt.Username = "112";
            opt.Password = "123456789Aa";
            opt.KeepAlive = true;
        });
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddFile("logs/asterisk.log");  
        });
        services.AddSingleton<IAsteriskMonitor, AsteriskMonitor>();
    })
    .Build();

var monitor = host.Services.GetRequiredService<IAsteriskMonitor>();

await monitor.StartAsync(CancellationToken.None);

Console.WriteLine("Monitoring incoming SIP calls. Press ENTER to exit...");
Console.ReadLine();

await monitor.StopAsync();