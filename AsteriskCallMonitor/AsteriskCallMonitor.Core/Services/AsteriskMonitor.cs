using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Asterisk.Manager;
using Sufficit.Asterisk.Manager.Events;

namespace AsteriskCallMonitor.Core.Services;

public class AsteriskMonitor : IAsteriskMonitor
{
    private readonly AMIProviderOptions _options;
    private readonly ILogger<AsteriskMonitor> _logger;
    private readonly ILogger<AsteriskManagerProvider> _providerLogger;

    private AsteriskManagerProvider? _provider;
    private IManagerConnection? _connection;
    private ManagerEventSubscriptions? _events;

    public AsteriskMonitor(
        IOptions<AMIProviderOptions> options,
        ILogger<AsteriskMonitor> logger,
        ILoggerFactory loggerFactory)     // <-- используем фабрику логгеров
    {
        _logger = logger;
        _providerLogger = loggerFactory.CreateLogger<AsteriskManagerProvider>(); // <-- правильный логгер
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connecting to Asterisk AMI...");

        _provider = new AsteriskManagerProvider(
            Options.Create(_options),
            _providerLogger
        );

        _connection = await _provider.ConnectAsync(keepalive: true);

        _events = new ManagerEventSubscriptions();
        _connection.Use(_events, disposable: true);

        _events.On<NewChannelEvent>(OnNewChannel);

        _logger.LogInformation("Asterisk monitor started.");
    }

    private void OnNewChannel(object? sender, NewChannelEvent evt)
    {
        if (string.IsNullOrEmpty(evt.CallerIdNum))
            return;

        if (evt.ChannelStateDesc == "Ring")
            _logger.LogInformation($"📞 Incoming call from: {evt.CallerIdNum}");
    }

    public async Task StopAsync()
    {
        if (_provider != null)
        {
            await _provider.DisconnectAsync();
            _provider.Dispose();
            _provider = null;
        }
    }
}