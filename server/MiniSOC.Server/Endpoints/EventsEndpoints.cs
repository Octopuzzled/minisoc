using MiniSOC.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MiniSOC.Server.Models;  

namespace MiniSOC.Server.Endpoints;

public static class EventsEndpoints
{
    public static void MapEventsEndpoints(this WebApplication app)
    {
        app.MapGet("/events", (
        [FromServices] IDatabaseService database,
        string? startTime,
        string? endTime,
        EventLevel? level,
        string? host) =>
    {
        var events = database.GetEvents(startTime, endTime, level, host);
        return Results.Ok(events);
    });
    }
}