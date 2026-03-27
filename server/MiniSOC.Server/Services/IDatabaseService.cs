using Microsoft.Data.Sqlite;
using MiniSOC.Server.Models;
using System.Collections.Generic;

namespace MiniSOC.Server.Services;

public interface IDatabaseService
{
    void Initialize();
    SqliteConnection GetConnection();

    bool AddEvent(Event @event);

    List<Event> GetAllEvents();

    int GetEventCount();

    List<Event> GetEvents(
    string? startTime = null,
    string? endTime = null,
    EventLevel? level = null,
    string? host = null
);
}