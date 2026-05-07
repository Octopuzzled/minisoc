using MiniSOC.Server.Services;
using MiniSOC.Server.Models;
using Microsoft.Extensions.Configuration;

namespace MiniSOC.Server.Tests;

/// <summary>
/// Tests for event persistence in SQLite database
/// </summary>
public class EventPersistenceTests
{
    [Fact]
    public void AddEvent_InsertsEventIntoDatabase()
    {
        // Arrange: Set up test database and service
        var testDbPath = "test_persistence.db";
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = $"Data Source={testDbPath}"
            })
            .Build();

        var dbService = new SqliteDatabaseService(config);
        dbService.Initialize();

        var testEvent = new Event
        {
            Timestamp = "2026-03-17T10:00:00Z",
            Host = "TEST-PC",
            Source = "TestSource",
            Level = EventLevel.Error,
            Message = "Test message"
        };
        
        // Act: Save event to database
        bool result = dbService.AddEvent(testEvent);

        // Assert: Verify event was saved successfully
        Assert.True(result);

        // Verify event data in database
        using var connection = dbService.GetConnection();
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT host, source, level, message FROM Events WHERE host = $host";
        command.Parameters.AddWithValue("$host", "TEST-PC");

        using var reader = command.ExecuteReader();
        Assert.True(reader.Read());

        Assert.Equal("TEST-PC", reader.GetString(0));
        Assert.Equal("TestSource", reader.GetString(1));
        Assert.Equal("Error", reader.GetString(2));
        Assert.Equal("Test message", reader.GetString(3));
        
        // Cleanup: Remove test database
        connection.Close();
        SqliteConnection.ClearAllPools(); 
        File.Delete(testDbPath);
    }

    [Fact]
    public void AddEvent_DuplicateEventId_ReturnsFalse()
    {
        // Arrange: Set up test database with explicit event ID
        var testDbPath = "test_duplicate.db";
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = $"Data Source={testDbPath}"
            })
            .Build();

        var dbService = new SqliteDatabaseService(config);
        dbService.Initialize();

        var testEvent = new Event
        {
            EventId = "test-123",
            Timestamp = "2026-03-17T10:00:00Z",
            Host = "TEST-PC",
            Source = "TestSource",
            Level = EventLevel.Error
        };
        
        // Act: Attempt to add same event twice
        bool firstAdd = dbService.AddEvent(testEvent);
        bool secondAdd = dbService.AddEvent(testEvent);

        // Assert: First insert succeeds, duplicate is rejected
        Assert.True(firstAdd);
        Assert.False(secondAdd);

        // Verify only one event exists in database
        using var connection = dbService.GetConnection();
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Events";
        var count = (long)command.ExecuteScalar();
        
        Assert.Equal(1, count);
        
        // Cleanup: Remove test database
        connection.Close();
        SqliteConnection.ClearAllPools();
        File.Delete(testDbPath);
    }
}