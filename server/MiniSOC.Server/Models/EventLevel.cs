using System.Text.Json.Serialization;

namespace MiniSOC.Server.Models;

/// <summary>
/// Severity level of a security event
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventLevel
{
    /// <summary>
    /// Informational event (lowest severity)
    /// </summary>
    Information = 0,
    
    /// <summary>
    /// Warning - potential issue
    /// </summary>
    Warning = 1,
    
    /// <summary>
    /// Error - confirmed issue
    /// </summary>
    Error = 2,
    
    /// <summary>
    /// Critical - severe issue requiring immediate attention
    /// </summary>
    Critical = 3
}