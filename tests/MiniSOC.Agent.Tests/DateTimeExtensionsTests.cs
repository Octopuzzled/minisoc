using Xunit;
using MiniSOC.Agent.Extensions;
using System;

namespace MiniSOC.Agent.Tests;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToSiemTimestamp_NormalizesToUtcWithZ()
    {
        // Arrange: Ein lokales Datum mit festem Versatz (z.B. 14:00 Uhr)
        // Wir simulieren eine Zeit, die nicht UTC ist
        DateTime? localTime = new DateTime(2026, 5, 7, 14, 0, 0, DateTimeKind.Local);

        // Act
        var result = localTime.ToSiemTimestamp();

        // Assert
        // 1. Prüfen, ob das Z am Ende steht (Akzeptanzkriterium #48)
        Assert.EndsWith("Z", result);
        
        // 2. Prüfen, ob das Format ISO-8601 entspricht (yyyy-MM-ddTHH:mm:ss...)
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}", result);
    }

    [Fact]
    public void ToSiemTimestamp_HandlesNullByUsingCurrentUtc()
    {
        // Arrange
        DateTime? nullTime = null;

        // Act
        var result = nullTime.ToSiemTimestamp();

        // Assert
        Assert.EndsWith("Z", result);
        // Prüfen, ob das Jahr aktuell ist, um zu sehen, ob DateTime.UtcNow gegriffen hat
        Assert.Contains(DateTime.UtcNow.Year.ToString(), result);
    }
}