using MiniSOC.Server.Services;
using MiniSOC.Server.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace MiniSOC.Server.Tests;

/// <summary>
/// Tests for event retrieval from SQLite database
/// </summary>
public class EventRetrievalTests : IDisposable
{
    private readonly string _testDbPath = "test_retrieval.db";

    public EventRetrievalTests()
    {
        CleanUpDatabase();
    }

    private SqliteDatabaseService CreateService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = $"Data Source={_testDbPath}"
            })
            .Build();
        return new SqliteDatabaseService(config);
    }

    [Fact]
    public void GetAllEvents_ReturnsStoredEvents()
    {
        // Arrange
        var dbService = CreateService();
        dbService.Initialize();

        dbService.AddEvent(new Event { Timestamp = "2026-03-18T10:00:00Z", Host = "TEST-PC-1", Source = "TestSource", Level = EventLevel.Error, Message = "First" });
        dbService.AddEvent(new Event { Timestamp = "2026-03-18T10:01:00Z", Host = "TEST-PC-2", Source = "TestSource", Level = EventLevel.Warning, Message = "Second", Channel = "Security" });
        dbService.AddEvent(new Event 
        { 
            Timestamp = "2026-03-18T10:02:00Z", Host = "TEST-PC-3", Source = "TestSource", Level = EventLevel.Information, Message = "Third", Provider = "TestProvider",
            Raw = new Dictionary<string, object> { ["testKey"] = "testValue", ["eventRecordId"] = 12345 }
        });

        // Act
        var events = dbService.GetAllEvents();

        // Assert
        Assert.Equal(3, events.Count);
        Assert.Equal("TEST-PC-1", events[0].Host);
        Assert.Equal("Security", events[1].Channel);
        Assert.Equal("TestProvider", events[2].Provider);
        Assert.Equal("testValue", ((JsonElement)events[2].Raw["testKey"]).GetString());
    }

    [Fact]
    public void GetEventCount_ReturnsCorrectCount()
    {
        // Arrange
        var dbService = CreateService();
        dbService.Initialize();
        dbService.AddEvent(new Event { Timestamp = "2026-03-18T10:00:00Z", Host = "PC-1", Source = "Test", Level = EventLevel.Information });
        dbService.AddEvent(new Event { Timestamp = "2026-03-18T10:05:00Z", Host = "PC-1", Source = "Test", Level = EventLevel.Information });

        // Act & Assert
        Assert.Equal(2, dbService.GetEventCount());
    }

    [Fact]
    public void GetEvents_WithFilters_ReturnsFilteredEvents()
    {
        // Arrange
        var dbService = CreateService();
        dbService.Initialize();

        dbService.AddEvent(new Event { Timestamp = "2026-03-18T10:00:00Z", Host = "PC-1", Source = "Test", Level = EventLevel.Error });
        dbService.AddEvent(new Event { Timestamp = "2026-03-18T11:00:00Z", Host = "PC-1", Source = "Test", Level = EventLevel.Warning });
        dbService.AddEvent(new Event { Timestamp = "2026-03-18T12:00:00Z", Host = "PC-2", Source = "Test", Level = EventLevel.Error, Provider = "TestProvider" });

        // Act & Assert
        
        // Filter by Level
        var errorEvents = dbService.GetEvents(level: EventLevel.Error);
        Assert.Equal(2, errorEvents.Count);

        // Filter by Host
        var pc1Events = dbService.GetEvents(host: "PC-1");
        Assert.Equal(2, pc1Events.Count);

        // Filter by Provider
        var providerEvents = dbService.GetEvents(provider: "TestProvider");
        Assert.Single(providerEvents);

        // Filter by Time Range
        var timeEvents = dbService.GetEvents(startTime: "2026-03-18T11:00:00Z");
        Assert.Equal(2, timeEvents.Count);

        // Combined Filter
        var combined = dbService.GetEvents(level: EventLevel.Error, host: "PC-1");
        Assert.Single(combined);
        Assert.Equal("PC-1", combined[0].Host);
    }

    public void Dispose()
    {
        CleanUpDatabase();
    }

    private void CleanUpDatabase()
    {
        // Force SQLite to release file locks
        SqliteConnection.ClearAllPools();
        if (File.Exists(_testDbPath))
        {
            try 
            { 
                File.Delete(_testDbPath); 
            } 
            catch (IOException) 
            {
                // Small delay to handle race conditions with the OS
                Thread.Sleep(100);
                try { File.Delete(_testDbPath); } catch { }
            }
        }
    }
}