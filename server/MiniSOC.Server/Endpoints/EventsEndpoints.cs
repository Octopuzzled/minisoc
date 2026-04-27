using MiniSOC.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MiniSOC.Server.Models;

namespace MiniSOC.Server.Endpoints;

/// <summary>
/// Event query endpoint configuration
/// </summary>
public static class EventsEndpoints
{
    /// <summary>
    /// Maps the event query endpoint to the application.
    /// Supports optional filtering by level, host, provider, and time range.
    /// </summary>
    public static void MapEventsEndpoints(this WebApplication app)
    {
        app.MapGet("/events", (
            [FromServices] IDatabaseService database,
            string? startTime,
            string? endTime,
            EventLevel? level,
            string? host,
            string? provider) =>
        {
            var events = database.GetEvents(startTime, endTime, level, host, provider);
            return Results.Ok(events);
        });
    }
}