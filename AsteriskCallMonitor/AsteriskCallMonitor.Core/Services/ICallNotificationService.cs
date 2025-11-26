
using AsteriskCallMonitor.Core.Models;

namespace AsteriskCallMonitor.Core.Services;

public interface ICallNotificationService
{
    Task NotifyAsync(CallEvent callEvent, CancellationToken ct = default);
}