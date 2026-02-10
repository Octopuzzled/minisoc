using System.Text;
using System.Text.Json;
using MiniSOC.Agent.Models;

namespace MiniSOC.Agent.Clients;

/// <summary>
/// Sends events to the server via HTTP POST.
/// Uses the /ingest endpoint as defined in API contract v0.1.
/// </summary>
public class HttpEventSender : IEventSender
{
    private readonly HttpClient _httpClient;
    private readonly string _serverUrl;
    
    /// <summary>
    /// Initializes a new instance of HttpEventSender.
    /// </summary>
    /// <param name="serverUrl">Base URL of the server (e.g., "http://localhost:5152")</param>
    public HttpEventSender(string serverUrl = "http://localhost:5152")
    {
        _httpClient = new HttpClient();
        _serverUrl = serverUrl.TrimEnd('/'); // Remove trailing slash if present
    }

    /// <summary>
    /// Sends events to the server's /ingest endpoint via HTTP POST.
    /// </summary>
    /// <param name="events">Events to send</param>
    /// <returns>True if server accepted all events (HTTP 200), false otherwise</returns>
    public async Task<bool> SendEventsAsync(IEnumerable<Event> events)
    {
        try
        {
            // Convert events to list for serialization
            var eventList = events.ToList();
            
            if (!eventList.Any())
            {
                Console.WriteLine("No events to send.");
                return true; // Nothing to send is not a failure
            }

            // Serialize events to JSON
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            var jsonContent = JsonSerializer.Serialize(eventList, jsonOptions);
            
            // Create HTTP request
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestUri = $"{_serverUrl}/ingest";
            
            Console.WriteLine($"Sending {eventList.Count} event(s) to {requestUri}...");
            
            // Send POST request
            var response = await _httpClient.PostAsync(requestUri, content);
            
            // Check response status
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✓ Events sent successfully. Server response: {responseBody}");
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✗ Server rejected events. Status: {response.StatusCode}, Response: {errorBody}");
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"✗ Network error: {ex.Message}");
            Console.WriteLine("  Make sure the server is running on the configured URL.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Unexpected error: {ex.Message}");
            return false;
        }
    }
}