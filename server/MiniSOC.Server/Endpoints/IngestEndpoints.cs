using Microsoft.AspNetCore.Mvc;
using MiniSOC.Server.Models;
using MiniSOC.Server.Services;
using System.Text.Json;

namespace MiniSOC.Server.Endpoints;

/// <summary>
/// Event ingestion endpoint configuration
/// </summary>
public static class IngestEndpoints
{
    /// <summary>
    /// Maps the ingest endpoint to the application.
    /// Accepts a single event or an array of events as JSON.
    /// Validates each event and returns a summary of accepted and rejected events.
    /// </summary>
    public static void MapIngestEndpoints(this WebApplication app)
    {
        app.MapPost("/ingest", async (
            HttpContext context,
            [FromServices] IDatabaseService database) =>
        {
            var response = new IngestResponse();
            
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                
                if (string.IsNullOrWhiteSpace(body))
                {
                    return Results.BadRequest(new IngestResponse
                    {
                        Rejected = 1,
                        Errors = new List<IngestError>
                        {
                            new() { Index = 0, Reason = "Request body is empty" }
                        }
                    });
                }
                
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };
                
                List<Event> events;
                
                if (body.TrimStart().StartsWith('['))
                {
                    events = JsonSerializer.Deserialize<List<Event>>(body, options) 
                             ?? new List<Event>();
                }
                else
                {
                    var singleEvent = JsonSerializer.Deserialize<Event>(body, options);
                    events = singleEvent != null ? new List<Event> { singleEvent } : new List<Event>();
                }
                
                for (int i = 0; i < events.Count; i++)
                {
                    var evt = events[i];
                    var validationErrors = ValidateEvent(evt);
                    
                    if (validationErrors.Any())
                    {
                        response = response with 
                        { 
                            Rejected = response.Rejected + 1,
                            Errors = response.Errors.Concat(validationErrors.Select(e => 
                                new IngestError { Index = i, Reason = e })).ToList()
                        };
                    }
                    else
                    {
                        bool addedToDatabase = database.AddEvent(evt);
                        if (addedToDatabase)
                        {
                            response = response with { Accepted = response.Accepted + 1 };
                        }
                        else
                        {
                            response = response with 
                            { 
                                Rejected = response.Rejected + 1,
                                Errors = response.Errors.Append(new IngestError 
                                { 
                                    Index = i, 
                                    Reason = "Event with this ID already exists" 
                                }).ToList()
                            };
                        }
                    }
                }
                
                return response.Rejected > 0 
                    ? Results.BadRequest(response) 
                    : Results.Ok(response);
            }
            catch (JsonException ex)
            {
                return Results.BadRequest(new IngestResponse
                {
                    Rejected = 1,
                    Errors = new List<IngestError>
                    {
                        new() { Index = 0, Reason = $"Invalid JSON: {ex.Message}" }
                    }
                });
            }
        })
        .WithName("IngestEvents")
        .WithOpenApi();        
    }
    
     /// <summary>
    /// Validates required fields on an event before storage.
    /// </summary>
    private static List<string> ValidateEvent(Event evt)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(evt.Timestamp))
            errors.Add("Timestamp is required");
            
        if (string.IsNullOrWhiteSpace(evt.Host))
            errors.Add("Host is required");
            
        if (string.IsNullOrWhiteSpace(evt.Source))
            errors.Add("Source is required");
        
        return errors;
    }
}