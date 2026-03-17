using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration; 

namespace MiniSOC.Server.Services;

public class SqliteDatabaseService : IDatabaseService
{
    // TODO: Felder (Connection String speichern)
    private readonly string _connectionString;
    
    // TODO: Konstruktor
    public SqliteDatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("EventsDatabase")
            ?? throw new InvalidOperationException("Connection string 'EventsDatabase' not found");
    }
    
    public void Initialize()
    {
        using var connection = GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Events (
            event_id TEXT NOT NULL PRIMARY KEY,
            timestamp TEXT NOT NULL,
            host TEXT NOT NULL,
            source TEXT NOT NULL,
            channel TEXT,
            provider TEXT,
            level TEXT NOT NULL,
            message TEXT,
            raw TEXT
            )";
        
        command.ExecuteNonQuery();
    }
    
    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}