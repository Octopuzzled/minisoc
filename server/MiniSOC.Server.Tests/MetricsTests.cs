using MiniSOC.Server.Services;
using MiniSOC.Server.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

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
}