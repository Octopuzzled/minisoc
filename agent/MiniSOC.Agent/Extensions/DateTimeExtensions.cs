namespace MiniSOC.Agent.Extensions;

public static class DateTimeExtensions
{
/// <summary>
/// Normalizes a DateTime to UTC and formats it for SIEM (ISO 8601 with Z suffix).
/// </summary>
    public static string ToSiemTimestamp(this DateTime? dt)
    {
        DateTime utcDate = dt?.ToUniversalTime() ?? DateTime.UtcNow;
        
        return utcDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}