using MiniSOC.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiniSOC.Server.Endpoints;

/// <summary>
/// Metrics endpoint configuration
/// </summary>
public static class MetricsEndpoints
{
    /// <summary>
    /// Maps the metrics endpoint to the application.
    /// Returns aggregated event statistics including counts, breakdowns, and trend data.
    /// </summary>
    public static void MapMetricsEndpoints(this WebApplication app)
    {
        app.MapGet("/metrics", (
            [FromServices] IMetricsService metrics) =>
        {
            var count = metrics.GetEventCount();
            var levels = metrics.GetEventsByLevel();
            var hosts = metrics.GetEventsByHost();
            var hours = metrics.GetEventsLast24h();
            var days = metrics.GetEventsLast7d();

            return Results.Ok(new
            {
                event_count = count,
                by_level = levels,
                by_host = hosts,
                trend = new
                {
                    last_24h = hours,
                    last_7d = days
                }
            });
        });
    }
}