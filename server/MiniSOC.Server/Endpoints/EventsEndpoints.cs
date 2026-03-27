using MiniSOC.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiniSOC.Server.Endpoints;

public static class EventsEndpoints
{
    public static void MapEventsEndpoints(this WebApplication app)
    {
        app.MapGet("/events", ([FromServices] IDatabaseService database) =>
        {
            var events = database.GetAllEvents();
            return Results.Ok(events);
        });
    }
}