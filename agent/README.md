# MiniSOC Agent

Event collection agent for the MiniSOC platform.  
Collects security events from configurable sources and transmits them to the MiniSOC server.

## Status
- ✅ Interface-based architecture (IEventSource)
- ✅ HTTP transmission to server (/ingest endpoint)
- ✅ DummyEventSource for development and testing
- ✅ Windows Event Log source (WindowsEventLogSource)
- ✅ Configuration system (appsettings.json)

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Running MiniSOC server (see `server/README.md`)
- **Windows required** for WindowsEventLogSource (run as Administrator)

### Run the agent

```bash
cd agent/MiniSOC.Agent
dotnet run
```

**Note:** On Windows, run as Administrator to access the Security event log channel.

**Expected output:**
MiniSOC Agent starting...
✓ Event source initialized (WindowsEventLogSource)
✓ Collected 5591 event(s)
✓ Event sender initialized (HTTP)
Sending 5591 event(s) to http://localhost:5152/ingest...
✓ Events sent successfully. Server response: {"accepted":5591,"rejected":0,"errors":[]}
========================================
✓ Agent completed successfully

### Build only (without running)

```bash
cd agent/MiniSOC.Agent
dotnet build
```

## How It Works

The agent follows a simple pipeline:

Event Source (IEventSource)
└─ Collects events from configured source
Event Collection
└─ GetEvents() returns IEnumerable<Event>
Event Sender (IEventSender)
└─ Sends events via HTTP POST to server
Result
└─ Returns success/failure status


### Current Implementation

**Event Source:** `WindowsEventLogSource`
- Reads real Windows Event Logs via `System.Diagnostics.Eventing.Reader`
- Channels and levels configurable via `appsettings.json`
- Requires Windows and Administrator privileges
- `DummyEventSource` available for development and testing on Linux

**Event Sender:** `HttpEventSender`
- Sends to configured server URL (default: `http://localhost:5152`)
- JSON serialization with camelCase naming
- Error handling for network issues
- Console feedback for development

## Architecture

### Clean Architecture with Interfaces

The agent uses interface-based design for flexibility and testability:
Models/
├─ Event.cs                    # Event schema (matches server schema v0.1)
└─ EventLevel.cs               # Severity enum
Services/
├─ IEventSource.cs             # Interface for event collection
├─ DummyEventSource.cs         # Development implementation (Linux)
└─ WindowsEventLogSource.cs    # Production implementation (Windows only)
Clients/
├─ IEventSender.cs             # Interface for event transmission
└─ HttpEventSender.cs          # HTTP POST implementation
Program.cs                       # Main entry point, wires everything together
appsettings.json                 # Configuration file

### Why Interfaces?

**IEventSource** allows pluggable event sources:
- `DummyEventSource` → Development/testing on Linux
- `WindowsEventLogSource` → Production (Windows only)
- `FileEventSource` → Offline mode (future)

**IEventSender** allows pluggable transport:
- `HttpEventSender` → HTTP POST (current)
- `MockEventSender` → Testing without server (future)

## Configuration

Configuration is loaded from `appsettings.json` at startup:

```json
{
  "Agent": {
    "ServerUrl": "http://localhost:5152",
    "IntervalSeconds": 30,
    "Channels": ["System", "Security", "Application"],
    "Levels": ["Warning", "Error", "Critical"]
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `ServerUrl` | MiniSOC server base URL | `http://localhost:5152` |
| `IntervalSeconds` | Collection interval in seconds | `30` |
| `Channels` | Windows Event Log channels to read | System, Security, Application |
| `Levels` | Minimum severity levels to collect | Warning, Error, Critical |

**Note:** `Information` level is excluded by default due to high volume.

## Event Schema

Events follow **schema v0.1** as defined in `docs/event-schema-v0.1.md`.

**Required fields:**
- `timestamp` (ISO 8601 format)
- `host` (hostname/machine identifier)
- `source` (event source, e.g., "WindowsEventLog")
- `level` (Information | Warning | Error | Critical)

**Optional fields:**
- `event_id` (generated if not provided)
- `channel` (e.g., "System", "Security")
- `provider` (e.g., "Service Control Manager")
- `message` (human-readable description)
- `raw` (source-specific additional data)

## Development

### Adding a New Event Source

1. Create a class implementing `IEventSource`:

```csharp
public class MyEventSource : IEventSource
{
    public IEnumerable<Event> GetEvents()
    {
        // Your implementation
    }
}
```

2. Update `Program.cs` to use your source:

```csharp
var source = new MyEventSource();
```

### Testing Without Server

The agent will report connection errors but won't crash:
✗ Network error: No connection could be made because the target machine actively refused it
Make sure the server is running on the configured URL.

Exit code will be `1` (failure) for scripting/automation.

## Technology Stack

- .NET 8.0 (Console Application)
- System.Text.Json (JSON serialization)
- System.Diagnostics.Eventing.Reader (Windows Event Log access)
- HttpClient (HTTP communication)
- Microsoft.Extensions.Configuration (configuration system)

## Next Steps

See `docs/backlog.md` for planned features.