using System.Text.Json.Serialization;

namespace MiniSOC.Server.Models;

/// <summary>
/// Response model for the event ingestion endpoint
/// </summary>
public record IngestResponse
{
    /// <summary>
    /// Number of events successfully stored
    /// </summary>
    [JsonPropertyName("accepted")]
    public int Accepted { get; init; }
    
    /// <summary>
    /// Number of events rejected due to validation errors or duplicates
    /// </summary>
    [JsonPropertyName("rejected")]
    public int Rejected { get; init; }
    
    /// <summary>
    /// List of errors for rejected events
    /// </summary>
    [JsonPropertyName("errors")]
    public List<IngestError> Errors { get; init; } = new();
}

/// <summary>
/// Describes a validation or storage error for a single event in an ingest request
/// </summary>
public record IngestError
{
    /// <summary>
    /// Zero-based index of the rejected event in the request array
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; init; }
    
    /// <summary>
    /// Human-readable description of the rejection reason
    /// </summary>
    [JsonPropertyName("reason")]
    public string Reason { get; init; } = string.Empty;
}