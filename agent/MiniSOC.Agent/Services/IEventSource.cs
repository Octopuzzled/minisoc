using MiniSOC.Agent.Models;

namespace MiniSOC.Agent.Services;

/// <summary>
/// Interface for event collection sources.
/// Implementations can collect events from various sources (dummy data, Windows Event Log, files, etc.)
/// </summary>
public interface IEventSource
{
    /// <summary>
    /// Retrieves events from the source.
    /// </summary>
    /// <returns>Collection of security events</returns>
    IEnumerable<Event> GetEvents();
}