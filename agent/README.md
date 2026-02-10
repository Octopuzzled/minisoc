# MiniSOC Agent

Event collection agent for the MiniSOC platform.  
Collects security events from configurable sources and transmits them to the MiniSOC server.

## Status
- ✅ Interface-based architecture (IEventSource)
- ✅ HTTP transmission to server (/ingest endpoint)
- ✅ DummyEventSource for development and testing
- ⏳ Windows Event Log source (Milestone 2 - planned)
- ⏳ Configuration system (planned)

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Running MiniSOC server (see `server/README.md`)

### Run the agent

```bash
cd agent/MiniSOC.Agent
dotnet run
```

**Expected output:**
```
MiniSOC Agent starting...

✓ Event source initialized (DummyEventSource)
✓ Collected 5 event(s)

✓ Event sender initialized (HTTP)

Sending 5 event(s) to http://localhost:5152/ingest...
✓ Events sent successfully. Server response: {"accepted":5,"rejected":0,"errors":[]}

========================================
✓ Agent completed successfully
========================================
```

### Build only (without running)

```bash
cd agent/MiniSOC.Agent
dotnet build
```

## How It Works

The agent follows a simple pipeline:

```
1. Event Source (IEventSource)
   └─ Collects events from configured source
   
2. Event Collection
   └─ GetEvents() returns IEnumerable<Event>
   
3. Event Sender (IEventSender)
   └─ Sends events via HTTP POST to server
   
4. Result
   └─ Returns success/failure status
```

### Current Implementation

**Event Source:** `DummyEventSource`
- Generates 5 realistic test events
- Covers all severity levels (Information, Warning, Error, Critical)
- Includes edge cases (long messages, stack traces)
- Events stored in memory for performance

**Event Sender:** `HttpEventSender`
- Sends to `http://localhost:5152/ingest` by default
- JSON serialization with camelCase naming
- Error handling for network issues
- Console feedback for development

## Architecture

### Clean Architecture with Interfaces

The agent uses interface-based design for flexibility and testability:

```
Models/
  ├─ Event.cs           # Event schema (matches server schema v0.1)
  └─ EventLevel.cs      # Severity enum

Services/
  ├─ IEventSource.cs           # Interface for event collection
  └─ DummyEventSource.cs       # Development implementation

Clients/
  ├─ IEventSender.cs           # Interface for event transmission
  └─ HttpEventSender.cs        # HTTP POST implementation

Program.cs              # Main entry point, wires everything together
```

### Why Interfaces?

**IEventSource** allows pluggable event sources:
- `DummyEventSource` → Development/testing on Linux
- `WindowsEventLogSource` → Production (Windows only, planned)
- `FileEventSource` → Offline mode (future)
- `TestEventSource` → Unit testing

**IEventSender** allows pluggable transport:
- `HttpEventSender` → HTTP POST (current)
- `MockEventSender` → Testing without server
- `QueueEventSender` → Message queue (future)

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

## Configuration

**Current:** Server URL is hardcoded to `http://localhost:5152`

**Planned:** Configuration file (`appsettings.json`) will support:
- Server URL
- Event source selection
- Batch size
- Retry logic

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

```
✗ Network error: No connection could be made because the target machine actively refused it
  Make sure the server is running on the configured URL.
```

Exit code will be `1` (failure) for scripting/automation.

## Technology Stack

- .NET 8.0 (Console Application)
- System.Text.Json (JSON serialization)
- HttpClient (HTTP communication)

## Next Steps

See `docs/backlog.md` for planned features:
- Windows Event Log integration (Milestone 2)
- Configuration system (Milestone 7)
- Retry logic and error handling
- Batch processing