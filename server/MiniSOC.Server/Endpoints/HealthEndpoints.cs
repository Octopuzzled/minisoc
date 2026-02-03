namespace MiniSOC.Server.Endpoints;

using MiniSOC.Server.Models;

/// <summary>
/// Health check endpoint configuration
/// </summary>
public static class HealthEndpoints
{
    /// <summary>
    /// Maps the health check endpoint to the application
    /// </summary>
    public static void MapHealthEndpoints(this WebApplication application)
    {
        application.MapGet("/health", () =>
        {
            return new HealthResponse(
                Status: "healthy",
                Timestamp: DateTime.UtcNow,
                Version: "0.1.0"
            );
        })
        .WithName("GetHealth")
        .WithOpenApi();
    }
}