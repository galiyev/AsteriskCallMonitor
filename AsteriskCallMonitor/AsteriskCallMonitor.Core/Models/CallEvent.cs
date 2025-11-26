// AsteriskCallMonitor.Core/Models/CallEvent.cs
namespace AsteriskCallMonitor.Core.Models;

public record CallEvent(
    string Direction,      // Incoming / Outgoing
    string CallerId,
    string CalledNumber,
    string Extension,
    string UniqueId,
    DateTime Timestamp,
    string Status
);