using System.Net;
using System.Text;
using System.Text.Json;
using MiniSOC.Server.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MiniSOC.Server.Tests;

/// <summary>
/// Integration tests for the event ingestion endpoint
/// </summary>
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
        // Arrange: Create a valid event payload
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

        // Act: Post the event to the ingest endpoint
        var response = await client.PostAsync("/ingest", content);

        // Assert: Verify successful ingestion
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Accepted);
        Assert.Equal(0, result.Rejected);
    }

    [Fact]
    public async Task PostIngest_EmptyBody_ReturnsBadRequest()
    {
        // Arrange: Prepare an empty request body
        var client = _factory.CreateClient();
        var content = new StringContent("", Encoding.UTF8, "application/json");

        // Act: Post the empty body
        var response = await client.PostAsync("/ingest", content);

        // Assert: Verify bad request response
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(0, result.Accepted);
        Assert.Equal(1, result.Rejected);
        Assert.Contains(result.Errors, e => e.Reason.ToLower().Contains("empty"));
    }

    [Fact]
    public async Task PostIngest_InvalidJson_ReturnsBadRequest()
    {
        // Arrange: Prepare an invalid JSON payload
        var client = _factory.CreateClient();
        var content = new StringContent("{invalid json", Encoding.UTF8, "application/json");

        // Act: Post the invalid JSON
        var response = await client.PostAsync("/ingest", content);

        // Assert: Verify bad request response due to format error
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IngestResponse>(json, _jsonOptions);
        
        Assert.NotNull(result);
        Assert.Equal(0, result.Accepted);
        Assert.Equal(1, result.Rejected);
    }
}