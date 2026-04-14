using MiniSOC.Server.Services;
using MiniSOC.Server.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MiniSOC.Server.Tests;

/// <summary>
/// Tests for event retrieval from SQLite database
/// </summary>
public class EventRetrievalTests
{
    [Fact]
    public void GetAllEvents_ReturnsStoredEvents()
    {
        // Arrange: Set up test database with three different event types
        var testDbPath = "test_retrieval.db";
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

        // Event 1: Minimal event with required fields only
        var event1 = new Event
        {
            Timestamp = "2026-03-18T10:00:00Z",
            Host = "TEST-PC-1",
            Source = "TestSource",
            Level = EventLevel.Error,
            Message = "First test event"
        };

        // Event 2: Event with nullable field (Channel)
        var event2 = new Event
        {
            Timestamp = "2026-03-18T10:01:00Z",
            Host = "TEST-PC-2",
            Source = "TestSource",
            Level = EventLevel.Warning,
            Message = "Second test event",
            Channel = "Security"
        };

        // Event 3: Event with Provider and Raw dictionary
        var event3 = new Event
        {
            Timestamp = "2026-03-18T10:02:00Z",
            Host = "TEST-PC-3",
            Source = "TestSource",
            Level = EventLevel.Information,
            Message = "Third test event",
            Provider = "TestProvider",
            Raw = new Dictionary<string, object>
            {
                ["testKey"] = "testValue",
                ["eventRecordId"] = 12345
            }
        };

        dbService.AddEvent(event1);
        dbService.AddEvent(event2);
        dbService.AddEvent(event3);
        
        // Act: Retrieve all events from database
        var events = dbService.GetAllEvents();
        
        // Assert: Verify correct count and data mapping
        Assert.Equal(3, events.Count);

        // Verify Event 1: Basic field mapping
        Assert.Equal("TEST-PC-1", events[0].Host);
        Assert.Equal("TestSource", events[0].Source);
        Assert.Equal(EventLevel.Error, events[0].Level);
        Assert.Equal("First test event", events[0].Message);
        Assert.Equal("2026-03-18T10:00:00Z", events[0].Timestamp);

        // Verify Event 2: Nullable field handling
        Assert.Equal("TEST-PC-2", events[1].Host);
        Assert.Equal(EventLevel.Warning, events[1].Level);
        Assert.Equal("Security", events[1].Channel);
        Assert.Null(events[1].Provider);

        // Verify Event 3: Dictionary deserialization from JSON
        Assert.Equal("TEST-PC-3", events[2].Host);
        Assert.Equal(EventLevel.Information, events[2].Level);
        Assert.Equal("TestProvider", events[2].Provider);
        Assert.NotNull(events[2].Raw);
        Assert.Equal(2, events[2].Raw.Count);
        Assert.Equal("testValue", ((JsonElement)events[2].Raw["testKey"]).GetString());
        Assert.Equal(12345, ((JsonElement)events[2].Raw["eventRecordId"]).GetInt32());
        
        // Cleanup: Remove test database
        File.Delete(testDbPath);
    }
    
    [Fact]
    public void GetEventCount_ReturnsCorrectCount()
    {
        // Arrange: Set up test database with three events
        var testDbPath = "test_count.db";
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

        var event1 = new Event
        {
            Timestamp = "2026-03-18T10:00:00Z",
            Host = "TEST-PC-1",
            Source = "TestSource",
            Level = EventLevel.Error,
            Message = "First test event"
        };

        var event2 = new Event
        {
            Timestamp = "2026-03-18T10:01:00Z",
            Host = "TEST-PC-2",
            Source = "TestSource",
            Level = EventLevel.Warning,
            Message = "Second test event",
            Channel = "Security"
        };

        var event3 = new Event
        {
            Timestamp = "2026-03-18T10:02:00Z",
            Host = "TEST-PC-3",
            Source = "TestSource",
            Level = EventLevel.Information,
            Message = "Third test event",
            Provider = "TestProvider",
            Raw = new Dictionary<string, object>
            {
                ["testKey"] = "testValue",
                ["eventRecordId"] = 12345
            }
        };

        dbService.AddEvent(event1);
        dbService.AddEvent(event2);
        dbService.AddEvent(event3);
        
        // Act: Get event count from database
        var count = dbService.GetEventCount();
        
        // Assert: Verify correct count
        Assert.Equal(3, count);
        
        // Cleanup: Remove test database
        File.Delete(testDbPath);
    }

    [Fact]
    public void GetEvents_WithFilters_ReturnsFilteredEvents()
    {
        // Arrange: Create test database with diverse events
        var testDbPath = "test_filters.db";
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

        // Add events with different timestamps, levels, hosts and providers
        dbService.AddEvent(new Event
        {
            Timestamp = "2026-03-18T09:00:00Z",
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Error
        });

        dbService.AddEvent(new Event
        {
            Timestamp = "2026-03-18T10:00:00Z",
            Host = "PC-2",
            Source = "Test",
            Level = EventLevel.Warning
        });

        dbService.AddEvent(new Event
        {
            Timestamp = "2026-03-18T11:00:00Z",
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Information
        });

         dbService.AddEvent(new Event
        {
            Timestamp = "2026-04-14T12:00:00Z",
            Host = "PC-2",
            Source = "Test",
            Level = EventLevel.Error,
            Provider = "TestProvider"
        });
        
        // Act & Assert: Tst different filters
        
        // Filter by level
        var errorEvents = dbService.GetEvents(level: EventLevel.Error);
        Assert.Equal(2, errorEvents.Count);
        Assert.Equal(EventLevel.Error, errorEvents[0].Level);
        
        // Filter by host
        var pc1Events = dbService.GetEvents(host: "PC-1");
        Assert.Equal(2, pc1Events.Count);
        Assert.All(pc1Events, e => Assert.Equal("PC-1", e.Host));

        //Filter by provider
        var providerEvents = dbService.GetEvents(provider: "TestProvider");
        Assert.Equal(1, providerEvents.Count);
        Assert.All(providerEvents, e => Assert.Equal("TestProvider", e.Provider));
        
        // Filter by time range
        var timeRangeEvents = dbService.GetEvents(startTime: "2026-03-18T10:00:00Z");
        Assert.Equal(3, timeRangeEvents.Count);
        
        // Combined filters
        var combinedEvents = dbService.GetEvents(level: EventLevel.Error, host: "PC-1");
        Assert.Equal(1, combinedEvents.Count);

        var combinedEventsProvider = dbService.GetEvents(level: EventLevel.Error, host: "PC-2", provider: "TestProvider");
        Assert.Equal(1, combinedEventsProvider.Count);
        
        // Cleanup
        File.Delete(testDbPath);
    }
}