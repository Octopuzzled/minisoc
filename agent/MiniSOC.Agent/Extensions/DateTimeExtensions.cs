namespace MiniSOC.Agent.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Normalisiert ein Datum zu UTC und formatiert es für das SIEM (ISO 8601 mit Z-Suffix).
    /// </summary>
    public static string ToSiemTimestamp(this DateTime? dt)
    {
        DateTime utcDate = dt?.ToUniversalTime() ?? DateTime.UtcNow;
        
        return utcDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}