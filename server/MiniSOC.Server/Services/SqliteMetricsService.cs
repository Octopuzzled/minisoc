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
}