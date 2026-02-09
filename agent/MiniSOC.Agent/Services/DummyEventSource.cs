using MiniSOC.Agent.Models;

namespace MiniSOC.Agent.Services;

/// <summary>
/// Dummy event source for development and testing.
/// Generates a fixed set of realistic test events covering different severity levels and scenarios.
/// </summary>
public class DummyEventSource : IEventSource
{
    private readonly List<Event> _events;

    public DummyEventSource()
    {
        // Initialize test events once (performance optimization)
        _events = new List<Event>
        {
            // Event 1: Information - Service started
            new Event
            {
                EventId = "evt-001",
                Timestamp = DateTime.UtcNow.AddMinutes(-10).ToString("o"),
                Host = "WEB-SERVER-01",
                Source = "WindowsEventLog",
                Channel = "System",
                Provider = "Service Control Manager",
                Level = EventLevel.Information,
                Message = "The Apache Web Server service entered the running state."
            },

            // Event 2: Warning - Disk space low
            new Event
            {
                EventId = "evt-002",
                Timestamp = DateTime.UtcNow.AddMinutes(-8).ToString("o"),
                Host = "DB-SERVER-02",
                Source = "WindowsEventLog",
                Channel = "System",
                Provider = "Disk",
                Level = EventLevel.Warning,
                Message = "Disk space is running low on volume C:. Only 5% free space remaining (2.3 GB of 50 GB)."
            },

            // Event 3: Error - Login failed
            new Event
            {
                EventId = "evt-003",
                Timestamp = DateTime.UtcNow.AddMinutes(-5).ToString("o"),
                Host = "WORKSTATION-15",
                Source = "WindowsEventLog",
                Channel = "Security",
                Provider = "Microsoft-Windows-Security-Auditing",
                Level = EventLevel.Error,
                Message = "An account failed to log on. Subject: Security ID: S-1-5-21-123456789. Account Name: john.doe. Logon Type: 3. Failure Reason: Unknown user name or bad password."
            },

            // Event 4: Critical - Firewall disabled
            new Event
            {
                EventId = "evt-004",
                Timestamp = DateTime.UtcNow.AddMinutes(-2).ToString("o"),
                Host = "GATEWAY-01",
                Source = "WindowsEventLog",
                Channel = "Security",
                Provider = "Microsoft-Windows-Windows Firewall With Advanced Security",
                Level = EventLevel.Critical,
                Message = "Windows Firewall has been disabled. The system is now vulnerable to network attacks."
            },

            // Event 5: Error - Application crash with long stack trace (edge case)
            new Event
            {
                EventId = "evt-005",
                Timestamp = DateTime.UtcNow.ToString("o"),
                Host = "APP-SERVER-03",
                Source = "WindowsEventLog",
                Channel = "Application",
                Provider = "Application Error",
                Level = EventLevel.Error,
                Message = "Application 'PaymentProcessor.exe' crashed unexpectedly. Exception details: System.NullReferenceException: Object reference not set to an instance of an object. at PaymentProcessor.Services.TransactionService.ProcessPayment(Payment payment) in C:\\Projects\\PaymentProcessor\\Services\\TransactionService.cs:line 142 at PaymentProcessor.Controllers.PaymentController.SubmitPayment(PaymentRequest request) in C:\\Projects\\PaymentProcessor\\Controllers\\PaymentController.cs:line 87"
            }
        };
    }

    /// <summary>
    /// Returns the fixed set of test events.
    /// Events are created once during construction for performance.
    /// </summary>
    /// <returns>Collection of 5 test security events</returns>
    public IEnumerable<Event> GetEvents()
    {
        return _events;
    }
}