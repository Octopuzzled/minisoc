namespace MiniSOC.Server.Models;

/// <summary>
/// Response model for the health check endpoint
/// </summary>
public record HealthResponse(
    string Status, 
    DateTime Timestamp, 
    string Version
);