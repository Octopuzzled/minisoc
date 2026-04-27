using Microsoft.Data.Sqlite;
using MiniSOC.Server.Models;

namespace MiniSOC.Server.Services;

/// <summary>
/// SQLite implementation of metrics service for aggregated event statistics
/// </summary>
public class SqliteMetricsService : IMetricsService
{
    private readonly IDatabaseService _database;

    public SqliteMetricsService(IDatabaseService database)
    {
        _database = database;
    }

    /// <summary>
    /// Returns the total number of stored events
    /// </summary>
    public int GetEventCount()
    {
        return _database.GetEventCount();
    }

    /// <summary>
    /// Returns event counts grouped by severity level
    /// </summary>
    public Dictionary<string, int> GetEventsByLevel()
    {
        var result = new Dictionary<string, int>();

        using var connection = _database.GetConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT level, COUNT(*) FROM Events GROUP BY level";
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var level = reader["level"].ToString();
            var count = (long)reader[1]; 
            result[level] = (int)count;
        }
        return result;
    }

    /// <summary>
    /// Returns event counts grouped by host
    /// </summary>
    public Dictionary<string, int> GetEventsByHost()
    {
        var result = new Dictionary<string, int>();

        using var connection = _database.GetConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT host, COUNT(*) FROM Events GROUP BY host";
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var host = reader["host"].ToString();
            var count = (long)reader[1]; 
            result[host] = (int)count;
        }
        return result;
    }

    /// <summary>
    /// Returns event counts in hourly buckets for the last 24 hours (UTC).
    /// Buckets with no events return count 0.
    /// </summary>
    public List<TrendBucket> GetEventsLast24h(DateTime? now = null)
    {
        var effectiveNow = now ?? DateTime.UtcNow;
        var result = new List<TrendBucket>();
        var dict = new Dictionary<string, int>();

        var end = new DateTime(effectiveNow.Year, effectiveNow.Month, effectiveNow.Day, effectiveNow.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);
        var start = end.AddHours(-24);
        
        using var connection = _database.GetConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT strftime('%Y-%m-%dT%H:00:00Z', timestamp) AS bucket, COUNT(*) FROM Events WHERE timestamp >= $start AND timestamp <= $end GROUP BY bucket";
        command.Parameters.AddWithValue("$start", start.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        command.Parameters.AddWithValue("$end", end.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var timeString = reader["bucket"].ToString();
            var count = (int)(long)reader[1];
            dict[timeString] = count;
        }

        // Fill all 24 buckets, using 0 for hours with no events
        while (start < end)
        {
            if (dict.ContainsKey(start.ToString("yyyy-MM-ddTHH:00:00Z")))
            {
                result.Add(new TrendBucket(start, dict[start.ToString("yyyy-MM-ddTHH:00:00Z")]));
            }
            else
            {
                result.Add(new TrendBucket(start, 0));
            }
            start = start.AddHours(1);
        }
        return result;
    }

    /// <summary>
    /// Returns event counts in daily buckets for the last 7 days (UTC).
    /// Buckets with no events return count 0.
    /// </summary>
    public List<TrendBucket> GetEventsLast7d(DateTime? now = null)
    {
        var result = new List<TrendBucket>();
        var dict = new Dictionary<string, int>();

        var effectiveNow = now ?? DateTime.UtcNow;
        var end = new DateTime(effectiveNow.Year, effectiveNow.Month, effectiveNow.Day, 0, 0, 0, DateTimeKind.Utc);
        var start = end.AddDays(-7);
        
        using var connection = _database.GetConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT strftime('%Y-%m-%d', timestamp) AS bucket, COUNT(*) FROM Events WHERE timestamp >= $start AND timestamp <= $end GROUP BY bucket";
        command.Parameters.AddWithValue("$start", start.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$end", end.ToString("yyyy-MM-dd"));
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var timeString = reader["bucket"].ToString();
            var count = (int)(long)reader[1];
            dict[timeString] = count;
        }

        // Fill all 7 buckets, using 0 for days with no events
        while (start < end)
        {
            if (dict.ContainsKey(start.ToString("yyyy-MM-dd")))
            {
                result.Add(new TrendBucket(start, dict[start.ToString("yyyy-MM-dd")]));
            }
            else
            {
                result.Add(new TrendBucket(start, 0));
            }
            start = start.AddDays(1);
        }
        return result;
    }
}