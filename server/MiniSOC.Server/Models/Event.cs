using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MiniSOC.Server.Models;

/// <summary>
/// Security event model matching schema v0.1
/// </summary>
public record Event
{
    /// <summary>
    /// Unique event identifier (UUID or hash)
    /// </summary>
    [JsonPropertyName("event_id")]
    public string? EventId { get; init; }
    
    /// <summary>
    /// Timestamp when the event occurred (ISO 8601 format)
    /// </summary>
    [Required(ErrorMessage = "Timestamp is required")]
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; } = string.Empty;
    
    /// <summary>
    /// Host machine identifier
    /// </summary>
    [Required(ErrorMessage = "Host is required")]
    [StringLength(255, ErrorMessage = "Host name must not exceed 255 characters")]
    [JsonPropertyName("host")]
    public string Host { get; init; } = string.Empty;
    
    /// <summary>
    /// Event source (e.g., "WindowsEventLog")
    /// </summary>
    [Required(ErrorMessage = "Source is required")]
    [StringLength(100, ErrorMessage = "Source must not exceed 100 characters")]
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;
    
    /// <summary>
    /// Event channel (optional, e.g., "System", "Security")
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; init; }
    
    /// <summary>
    /// Event provider (optional, e.g., "Service Control Manager")
    /// </summary>
    [JsonPropertyName("provider")]
    public string? Provider { get; init; }
    
    /// <summary>
    /// Event severity level
    /// </summary>
    [Required(ErrorMessage = "Level is required")]
    [JsonPropertyName("level")]
    public EventLevel Level { get; init; }
    
    /// <summary>
    /// Human-readable event message
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
    
    /// <summary>
    /// Raw event data (flexible, source-specific)
    /// </summary>
    [JsonPropertyName("raw")]
    public Dictionary<string, object>? Raw { get; init; }
}