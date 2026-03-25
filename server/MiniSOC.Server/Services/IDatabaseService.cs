using Microsoft.Data.Sqlite;
using MiniSOC.Server.Models;

namespace MiniSOC.Server.Services;

public interface IDatabaseService
{
    void Initialize();
    SqliteConnection GetConnection();

    bool AddEvent(Event @event);
}