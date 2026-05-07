namespace MiniSOC.Server.Tests;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MiniSOC.Server.Models;

/// <summary>
/// Integration tests for the health check endpoint
/// </summary>
public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var healthResponse = JsonSerializer.Deserialize<HealthResponse>(json, options);

        Assert.NotNull(healthResponse);
        Assert.Equal("healthy", healthResponse.Status);
        Assert.Equal("0.1.0", healthResponse.Version);
        
        // Verify timestamp is within a reasonable range (last 2 minutes)
        Assert.InRange(healthResponse.Timestamp, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }
}