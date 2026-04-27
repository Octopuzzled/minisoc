using MiniSOC.Server.Services;
using Microsoft.Extensions.Configuration;

namespace MiniSOC.Server.Tests;

/// <summary>
/// Tests for database initialization and schema creation
/// </summary>
public class DatabaseServiceTests
{
    [Fact]
    public void Initialize_CreatesDatabase()
    {
        // Arrange
        var testDbPath = "test_events.db";
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:EventsDatabase"] = "Data Source=test_events.db"
            })
            .Build();
        var dbService = new SqliteDatabaseService(config);
        
        // Act
        dbService.Initialize();
        
        // Assert
        Assert.True(File.Exists("test_events.db"));

        using var connection = dbService.GetConnection();
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Events'";
        var result = command.ExecuteScalar();

        Assert.Equal("Events", result);

        // Cleanup
        connection.Close();
        File.Delete("test_events.db");
    }
}