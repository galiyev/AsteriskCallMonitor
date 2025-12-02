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
    .UseSerilog()
    .ConfigureServices(services =>
    {
        // Именованный профиль "test"
        // services.Configure<AMIProviderOptions>("test", opt =>
        // {
        //     opt.Address = "10.10.111.8";
        //     opt.Port = 5038;
        //     opt.Username = "adminasterisk";
        //     opt.Password = "KB+Fu4D$zF(";
        //     opt.KeepAlive = true;
        // });
        
        services.Configure<AMIProviderOptions>("test", opt =>
        {
            opt.Address = "127.0.0.1";   // Docker → хостовая машина
            opt.Port = 5038;             // Проброшенный AMI порт
            opt.Username = "ami";        // Имя пользователя из FreePBX
            opt.Password = "ami123";     // Пароль из FreePBX
            opt.KeepAlive = true;
        });

        
        
        // Основной сервис мониторинга
        services.AddSingleton<IAsteriskMonitor, AsteriskMonitor>();
    })
    .Build();

var monitor = host.Services.GetRequiredService<IAsteriskMonitor>();

await monitor.StartAsync(CancellationToken.None);

Console.WriteLine("Monitoring incoming SIP calls. Press ENTER to exit...");
Console.ReadLine();

await monitor.StopAsync();