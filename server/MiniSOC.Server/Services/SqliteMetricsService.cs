using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration; 
using MiniSOC.Server.Models;
using System.Text.Json;

namespace MiniSOC.Server.Services;

/// <summary>
/// Service for metrics used in dashboard
/// </summary>
/// 
public class SqliteMetricsService : IMetricsService
{
    private readonly IDatabaseService _database;

    public SqliteMetricsService(IDatabaseService database)
    {
        _database = database;
    }

    public int GetEventCount()
    {
        return _database.GetEventCount();
    }

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

    public List<TrendBucket> GetEventsLast24h()
    {
        var result = new List<TrendBucket>();
        var dict = new Dictionary<string, int>();

        var now = DateTime.UtcNow;
        var end = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);
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

        public List<TrendBucket> GetEventsLast7d()
    {
        var result = new List<TrendBucket>();
        var dict = new Dictionary<string, int>();

        var now = DateTime.UtcNow;
        var end = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
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