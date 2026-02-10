using MiniSOC.Agent.Models;

namespace MiniSOC.Agent.Clients;

/// <summary>
/// Interface for sending events to the server.
/// Implementations can use different transport mechanisms (HTTP, file, queue, etc.)
/// </summary>
public interface IEventSender
{
    /// <summary>
    /// Sends events to the server asynchronously.
    /// </summary>
    /// <param name="events">Events to send</param>
    /// <returns>True if all events were accepted, false otherwise</returns>
    Task<bool> SendEventsAsync(IEnumerable<Event> events);
}