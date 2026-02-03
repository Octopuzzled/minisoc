using System.Collections.Concurrent;
using MiniSOC.Server.Models;

namespace MiniSOC.Server.Services;

/// <summary>
/// Interface for event storage operations
/// </summary>
public interface IEventStorageService
{
    /// <summary>
    /// Add an event to storage
    /// </summary>
    /// <param name="event">The event to store</param>
    /// <returns>True if added successfully, false if event_id already exists</returns>
    bool AddEvent(Event @event);
    
    /// <summary>
    /// Retrieve all stored events
    /// </summary>
    /// <returns>Collection of all events</returns>
    IEnumerable<Event> GetAllEvents();
    
    /// <summary>
    /// Get count of stored events
    /// </summary>
    int GetEventCount();
}

/// <summary>
/// In-memory storage for security events using thread-safe dictionary
/// </summary>
public class EventStorageService : IEventStorageService
{
    // Why ConcurrentDictionary?
    // - Thread-safe (multiple POST requests)
    // - Fast lookup by event_id
    // - No locking needed
    private readonly ConcurrentDictionary<string, Event> _events = new();

    public bool AddEvent(Event @event)
    {
        // Generate event_id if not provided
        var eventId = @event.EventId ?? Guid.NewGuid().ToString();
        
        // Create new event with guaranteed event_id
        var eventWithId = @event with { EventId = eventId };
        
        // TryAdd returns false if key already exists (thread-safe)
        return _events.TryAdd(eventId, eventWithId);
    }

    public IEnumerable<Event> GetAllEvents()
    {
        return _events.Values;
    }

    public int GetEventCount()
    {
        return _events.Count;
    }
}