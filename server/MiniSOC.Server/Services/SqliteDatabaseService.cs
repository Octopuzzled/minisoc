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

    public int GetEventCount()
    {
        using var connection = GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Events";
        var count = (long)command.ExecuteScalar();
        return (int)count;
    }

    public List<Event> GetAllEvents()
    {
        var events = new List<Event>();

        using var connection = GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Events";
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            // Map database row to Event object
            var event_id = reader["event_id"].ToString();
            var timestamp = reader["timestamp"].ToString();
            var host = reader["host"].ToString();
            var source = reader["source"].ToString();
            var channel = reader["channel"] == DBNull.Value ? null : reader["channel"].ToString();
            var provider = reader["provider"] == DBNull.Value ? null : reader["provider"].ToString();
            var levelString = reader["level"].ToString();
            var level = Enum.Parse<EventLevel>(levelString);
            var message = reader["message"] == DBNull.Value ? null : reader["message"].ToString();
            var rawJson = reader["raw"] == DBNull.Value ? null : reader["raw"].ToString();
            var raw = rawJson != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(rawJson) : null;
        
            var evt = new Event
            {
                EventId = event_id,
                Timestamp = timestamp,
                Host = host,
                Source = source,
                Channel = channel,
                Provider = provider,
                Level = level,
                Message = message,
                Raw = raw
            };
            events.Add(evt);
        }
        return events;        
    }

    public List<Event> GetEvents(
        string? startTime = null,
        string? endTime = null,
        EventLevel? level = null,
        string? host = null)
    {
        var events = new List<Event>();
        
        using var connection = GetConnection();
        connection.Open();
        
        var command = connection.CreateCommand();
        
        // Build WHERE clause dynamically based on provided filters
        var conditions = new List<string>();
        
        if (startTime != null)
        {
            conditions.Add("timestamp >= $startTime");
            command.Parameters.AddWithValue("$startTime", startTime);
        }

        if (endTime != null)
        {
            conditions.Add("timestamp <= $endTime");
            command.Parameters.AddWithValue("$endTime", endTime);
        }

        if (level != null)
        {
            conditions.Add("level = $level");
            command.Parameters.AddWithValue("$level", level.ToString());
        }

        if (host != null)
        {
            conditions.Add("host = $host");
            command.Parameters.AddWithValue("$host", host);
        }

        var whereClause = conditions.Count > 0 
            ? "WHERE " + string.Join(" AND ", conditions)
            : "";
        
        command.CommandText = $"SELECT * FROM Events {whereClause}";
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            // Map database row to Event object (using unique variable names to avoid parameter shadowing)
            var eventIdValue = reader["event_id"].ToString();
            var timestampValue = reader["timestamp"].ToString();
            var hostValue = reader["host"].ToString();
            var sourceValue = reader["source"].ToString();
            var channelValue = reader["channel"] == DBNull.Value ? null : reader["channel"].ToString();
            var providerValue = reader["provider"] == DBNull.Value ? null : reader["provider"].ToString();
            var levelStringValue = reader["level"].ToString();
            var levelValue = Enum.Parse<EventLevel>(levelStringValue);
            var messageValue = reader["message"] == DBNull.Value ? null : reader["message"].ToString();
            var rawJsonValue = reader["raw"] == DBNull.Value ? null : reader["raw"].ToString();
            var rawValue = rawJsonValue != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(rawJsonValue) : null;
        
            var evt = new Event
            {
                EventId = eventIdValue,
                Timestamp = timestampValue,
                Host = hostValue,
                Source = sourceValue,
                Channel = channelValue,
                Provider = providerValue,
                Level = levelValue,
                Message = messageValue,
                Raw = rawValue
            };
            events.Add(evt);
        }        
        return events;
    }
}