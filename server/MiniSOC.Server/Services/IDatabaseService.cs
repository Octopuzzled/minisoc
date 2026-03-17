using Microsoft.Data.Sqlite;

namespace MiniSOC.Server.Services;

public interface IDatabaseService
{
    void Initialize ();
    SqliteConnection GetConnection();
}