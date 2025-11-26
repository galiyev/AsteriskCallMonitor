
using AsteriskCallMonitor.Core.Models;

namespace AsteriskCallMonitor.Core.Services;

public interface IAsteriskMonitor
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();
}
