using MiniSOC.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MiniSOC.Server.Models;  

namespace MiniSOC.Server.Endpoints;

public static class MetricsEndpoints
{
    public static void MapMetricsEndpoints(this WebApplication app)
    {
        app.MapGet("/metrics", (
        [FromServices] IMetricsService metrics) =>
        {
            var count = metrics.GetEventCount();
            var levels = metrics.GetEventsByLevel();
            var hosts = metrics.GetEventsByHost();

            return Results.Ok(new
            {
                event_count = count,
                by_level = levels,
                by_host = hosts
            });
        });

    }
}