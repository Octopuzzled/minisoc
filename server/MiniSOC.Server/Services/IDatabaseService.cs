using Microsoft.Data.Sqlite;
using MiniSOC.Server.Models;

namespace MiniSOC.Server.Services;

/// <summary>
/// Defines the contract for database operations on events
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Creates the database schema if it does not already exist
    /// </summary>
    void Initialize();

    /// <summary>
    /// Returns an open-ready SQLite connection
    /// </summary>
    SqliteConnection GetConnection();

    /// <summary>
    /// Stores a single event. Returns false if the event ID already exists.
    /// </summary>
    bool AddEvent(Event @event);

    /// <summary>
    /// Returns all events without filtering
    /// </summary>
    List<Event> GetAllEvents();

    /// <summary>
    /// Returns the total number of stored events
    /// </summary>
    int GetEventCount();

    /// <summary>
    /// Returns events matching the given filters. All parameters are optional.
    /// </summary>
    List<Event> GetEvents(
        string? startTime = null,
        string? endTime = null,
        EventLevel? level = null,
        string? host = null,
        string? provider = null
    );
}