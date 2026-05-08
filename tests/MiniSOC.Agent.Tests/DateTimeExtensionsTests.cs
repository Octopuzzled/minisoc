using MiniSOC.Agent.Extensions;
using System;

namespace MiniSOC.Agent.Tests;

/// <summary>
/// Tests for DateTime extension methods used in event timestamp normalization.
/// </summary>
public class DateTimeExtensionsTests
{
    [Fact]
    public void ToSiemTimestamp_NormalizesToUtcWithZ()
    {
        // Arrange: A local date with fixed offset (e.g. 14:00)
        // We simulate a time that is not UTC
        DateTime? localTime = new DateTime(2026, 5, 7, 14, 0, 0, DateTimeKind.Local);

        // Act
        var result = localTime.ToSiemTimestamp();

        // Assert
        // 1. Verify the Z suffix is present (acceptance criterion #48)
        Assert.EndsWith("Z", result);
        
        // 2. Verify the format matches ISO-8601 (yyyy-MM-ddTHH:mm:ss...)
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
        // Verify the year is current to confirm DateTime.UtcNow was used
        Assert.Contains(DateTime.UtcNow.Year.ToString(), result);
    }
}