using System.Text.Json.Serialization;

namespace MiniSOC.Server.Models;

public record IngestResponse
{
    [JsonPropertyName("accepted")]
    public int Accepted { get; init; }
    
    [JsonPropertyName("rejected")]
    public int Rejected { get; init; }
    
    [JsonPropertyName("errors")]
    public List<IngestError> Errors { get; init; } = new();
}

public record IngestError
{
    [JsonPropertyName("index")]
    public int Index { get; init; }
    
    [JsonPropertyName("reason")]
    public string Reason { get; init; } = string.Empty;
}