using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration; 
using MiniSOC.Server.Models;
using System.Text.Json;

namespace MiniSOC.Server.Services;

/// <summary>
/// SQLite implementation of database service for event storage
/// </summary>
public class SqliteDatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    
    public SqliteDatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("EventsDatabase")
            ?? throw new InvalidOperationException("Database connection string not configured");
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

    public bool AddEvent(Event @event)
    {
        // Generate event_id if not provided
        var eventId = @event.EventId ?? Guid.NewGuid().ToString();
        var eventWithId = @event with { EventId = eventId };
        
        using var connection = GetConnection();
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Events (
                event_id, timestamp, host, source, 
                channel, provider, level, message, raw
            ) VALUES (
                $event_id, $timestamp, $host, $source,
                $channel, $provider, $level, $message, $raw
            )";
        
        // Bind parameters (prevents SQL injection)
        command.Parameters.AddWithValue("$event_id", eventWithId.EventId);
        command.Parameters.AddWithValue("$timestamp", eventWithId.Timestamp);
        command.Parameters.AddWithValue("$host", eventWithId.Host);
        command.Parameters.AddWithValue("$source", eventWithId.Source);
        command.Parameters.AddWithValue("$channel", eventWithId.Channel ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$provider", eventWithId.Provider ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$level", eventWithId.Level.ToString());
        command.Parameters.AddWithValue("$message", eventWithId.Message ?? (object)DBNull.Value);
        
        // Serialize dictionary to JSON for storage
        var rawJson = eventWithId.Raw != null 
            ? JsonSerializer.Serialize(eventWithId.Raw) 
            : null;
        command.Parameters.AddWithValue("$raw", rawJson ?? (object)DBNull.Value);
        
        try
        {
            command.ExecuteNonQuery();
            return true;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            // Duplicate primary key (SQLITE_CONSTRAINT)
            return false;
        }
    }
}