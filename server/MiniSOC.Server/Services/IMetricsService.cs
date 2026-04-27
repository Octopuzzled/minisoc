using MiniSOC.Server.Models;

namespace MiniSOC.Server.Services;

/// <summary>
/// Defines the contract for aggregated event metrics
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Returns the total number of stored events
    /// </summary>
    int GetEventCount();

    /// <summary>
    /// Returns event counts grouped by severity level
    /// </summary>
    Dictionary<string, int> GetEventsByLevel();

    /// <summary>
    /// Returns event counts grouped by host
    /// </summary>
    Dictionary<string, int> GetEventsByHost();

    /// <summary>
    /// Returns event counts in hourly buckets for the last 24 hours
    /// </summary>
    List<TrendBucket> GetEventsLast24h();

    /// <summary>
    /// Returns event counts in daily buckets for the last 7 days
    /// </summary>
    List<TrendBucket> GetEventsLast7d();
}