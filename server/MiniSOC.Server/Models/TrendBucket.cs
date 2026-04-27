namespace MiniSOC.Server.Models;

/// <summary>
/// Represents a single time bucket in a trend metric,
/// containing a timestamp and the event count for that period.
/// </summary>
public record TrendBucket(DateTime Time, int Count);