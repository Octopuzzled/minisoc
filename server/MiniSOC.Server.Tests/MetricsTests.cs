using MiniSOC.Server.Services;
using MiniSOC.Server.Models;
using Microsoft.Extensions.Configuration;

namespace MiniSOC.Server.Tests;

/// <summary>
/// Tests for metrics
/// </summary>
public class MetricsTests
{
    [Fact]
    public void GetMetrics_ReturnsAggregatedData()
    {
        // Arrange: Create test database with diverse events
        var testDbPath = "test_metrics.db";
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = $"Data Source={testDbPath}"
            })
            .Build();

        var dbService = new SqliteDatabaseService(config);
        var metricsService = new SqliteMetricsService(dbService);
        dbService.Initialize();

        // Add events with different levels and hosts
        dbService.AddEvent(new Event
        {
            Timestamp = "2026-04-14T09:00:00Z",
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Warning
        });

        dbService.AddEvent(new Event
        {
            Timestamp = "2026-04-14T10:00:00Z",
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Information
        });

        dbService.AddEvent(new Event
        {
            Timestamp = "2026-04-14T11:00:00Z",
            Host = "PC-2",
            Source = "Test",
            Level = EventLevel.Warning
        });

        // Act & Assert
        var countDB = dbService.GetEventCount();
        var countMetrics = metricsService.GetEventCount();
        Assert.Equal(countDB, countMetrics);

        var levels = metricsService.GetEventsByLevel();
        Assert.Equal(2, levels["Warning"]);
        Assert.Equal(1, levels["Information"]);

        var hosts = metricsService.GetEventsByHost();
        Assert.Equal(2, hosts["PC-1"]);
        Assert.Equal(1, hosts["PC-2"]);

        // Cleanup
        File.Delete(testDbPath);
    }

    [Fact]
    public void GetMetrics_24h()
    {
        // Arrange: Create test database with diverse events
        var testDbPath = "test_metrics_24h.db";
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = $"Data Source={testDbPath}"
            })
            .Build();

        var dbService = new SqliteDatabaseService(config);
        var metricsService = new SqliteMetricsService(dbService);
        dbService.Initialize();

        //Add events with different timestamps and some with same timestamp
        var now = DateTime.UtcNow;
        var timestamp1 = now.AddMinutes(-30).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var timestamp2 = now.AddHours(-3).ToString("yyyy-MM-ddTHH:mm:ssZ");

        dbService.AddEvent(new Event
        {
            Timestamp = timestamp1,
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Warning
        });

        dbService.AddEvent(new Event
        {
            Timestamp = timestamp1,
            Host = "PC-2",
            Source = "Test",
            Level = EventLevel.Error
        });

        dbService.AddEvent(new Event
        {
            Timestamp = timestamp2,
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Information
        });

        // Act & Assert
        var events = metricsService.GetEventsLast24h();
        Assert.Equal(24, events.Count);

        var expectedHour1 = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(-1);
        var bucket1 = events.First(b => b.Time == expectedHour1);
        Assert.Equal(2, bucket1.Count);

        var expectedHour2 = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(-3);
        var bucket2 = events.First(b => b.Time == expectedHour2);
        Assert.Equal(1, bucket2.Count);
    }

    [Fact]
    public void GetMetrics_7d()
    {
        // Arrange: Create test database with diverse events
        var testDbPath = "test_metrics_7d.db";
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = $"Data Source={testDbPath}"
            })
            .Build();

        var dbService = new SqliteDatabaseService(config);
        var metricsService = new SqliteMetricsService(dbService);
        dbService.Initialize();

        //Add events with different timestamps and some with same timestamp
        var timestamp = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        dbService.AddEvent(new Event
        {
            Timestamp = timestamp,
            Host = "PC-1",
            Source = "Test",
            Level = EventLevel.Warning
        });

        //Act & Assert
        var events = metricsService.GetEventsLast7d();
        Assert.Equal(7, events.Count);

        var expectedDay = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0).AddDays(-1);
        var bucket = events.First(b => b.Time == expectedDay);
        Assert.Equal(1, bucket.Count);
    }
}