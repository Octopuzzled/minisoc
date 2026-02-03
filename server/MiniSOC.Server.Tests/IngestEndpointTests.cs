using System.Net;
using System.Text;
using System.Text.Json;
using MiniSOC.Server.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MiniSOC.Server.Tests;

public class IngestEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public IngestEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
    }

    [Fact]
    public async Task PostIngest_ValidEvent_ReturnsOkWithAccepted()
    {
        // Arrange
        var client = _factory.CreateClient();
        var validEvent = new
        {
            timestamp = "2026-02-03T16:00:00Z",
            host = "TEST-PC",
            source = "TestSource",
            level = "Error",
            message = "Test event"
        };
        var content = new StringContent(
            JsonSerializer.Serialize(validEvent),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/ingest", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Accepted);
        Assert.Equal(0, result.Rejected);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task PostIngest_InvalidEvent_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidEvent = new
        {
            host = "TEST-PC",
            level = "Error"
        };
        var content = new StringContent(
            JsonSerializer.Serialize(invalidEvent),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/ingest", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(0, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Reason.Contains("Timestamp"));
        Assert.Contains(result.Errors, e => e.Reason.Contains("Source"));
    }

    [Fact]
    public async Task PostIngest_ArrayOfEvents_AcceptsMultiple()
    {
        // Arrange
        var client = _factory.CreateClient();
        var events = new[]
        {
            new
            {
                timestamp = "2026-02-03T16:00:00Z",
                host = "PC-1",
                source = "TestSource",
                level = "Error"
            },
            new
            {
                timestamp = "2026-02-03T16:01:00Z",
                host = "PC-2",
                source = "TestSource",
                level = "Warning"
            }
        };
        var content = new StringContent(
            JsonSerializer.Serialize(events),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/ingest", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Accepted);
        Assert.Equal(0, result.Rejected);
    }

    [Fact]
    public async Task PostIngest_MixedValidInvalid_ReturnsPartialSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var events = new object[]
        {
            new
            {
                timestamp = "2026-02-03T16:00:00Z",
                host = "PC-1",
                source = "TestSource",
                level = "Error"
            },
            new
            {
                host = "PC-2",
                level = "Warning"
            }
        };
        var content = new StringContent(
            JsonSerializer.Serialize(events),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync("/ingest", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.Equal(2, result.Errors.Count);
        Assert.All(result.Errors, e => Assert.Equal(1, e.Index));
    }

    [Fact]
    public async Task PostIngest_EmptyBody_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new StringContent("", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/ingest", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(0, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.Contains(result.Errors, e => e.Reason.Contains("empty"));
    }

    [Fact]
    public async Task PostIngest_InvalidJson_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new StringContent("{invalid json", Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/ingest", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Contains(result.Errors, e => e.Reason.Contains("Invalid JSON"));
    }
}