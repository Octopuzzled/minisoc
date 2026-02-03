# MiniSOC Server

REST API for the MiniSOC log collection platform.

## Status
- ✅ Health check endpoint implemented
- ✅ Event ingestion endpoint implemented (in-memory storage)
- ⏳ Query endpoints (planned)
- ⏳ Persistence layer (planned)

## Quick Start

### Run the server
```bash
cd server/MiniSOC.Server
dotnet run
```

Server starts on: `http://localhost:5152`

### Available Endpoints

#### GET /health
Health check endpoint.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-03T15:30:00Z",
  "version": "0.1.0"
}
```

**Status Codes:**
- `200 OK` - Service is healthy

---

#### POST /ingest
Ingest security events for storage and analysis.

**Request Body:**

Single event:
```json
{
  "timestamp": "2026-02-03T16:00:00Z",
  "host": "DESKTOP-123",
  "source": "WindowsEventLog",
  "level": "Error",
  "message": "Login failed",
  "channel": "Security",
  "provider": "Microsoft-Windows-Security-Auditing"
}
```

Array of events:
```json
[
  {
    "timestamp": "2026-02-03T16:00:00Z",
    "host": "DESKTOP-123",
    "source": "WindowsEventLog",
    "level": "Error"
  },
  {
    "timestamp": "2026-02-03T16:01:00Z",
    "host": "SERVER-456",
    "source": "WindowsEventLog",
    "level": "Warning"
  }
]
```

**Required Fields:**
- `timestamp` (ISO 8601 format)
- `host` (hostname/machine identifier)
- `source` (event source, e.g., "WindowsEventLog")
- `level` (one of: Information, Warning, Error, Critical)

**Optional Fields:**
- `event_id` (generated if not provided)
- `message`
- `channel`
- `provider`
- `raw` (arbitrary key-value data)

**Response:**
```json
{
  "accepted": 2,
  "rejected": 0,
  "errors": []
}
```

**Status Codes:**
- `200 OK` - All events accepted
- `400 Bad Request` - Validation errors or malformed JSON

**Error Response Example:**
```json
{
  "accepted": 1,
  "rejected": 1,
  "errors": [
    {
      "index": 1,
      "reason": "Timestamp is required"
    }
  ]
}
```

---

### Test the API

**Browser:**
- Swagger UI: `http://localhost:5152/swagger`
- Health endpoint: `http://localhost:5152/health`

**curl examples:**
```bash
# Health check
curl http://localhost:5152/health

# Ingest single event
curl -X POST http://localhost:5152/ingest \
  -H "Content-Type: application/json" \
  -d '{"timestamp":"2026-02-03T16:00:00Z","host":"PC-123","source":"WindowsEventLog","level":"Error"}'

# Ingest multiple events
curl -X POST http://localhost:5152/ingest \
  -H "Content-Type: application/json" \
  -d '[{"timestamp":"2026-02-03T16:00:00Z","host":"PC-1","source":"WindowsEventLog","level":"Error"},{"timestamp":"2026-02-03T16:01:00Z","host":"PC-2","source":"WindowsEventLog","level":"Warning"}]'
```

## Development

### Run tests
```bash
cd server/MiniSOC.Server.Tests
dotnet test
```

### Project Structure
```
server/
├── MiniSOC.Server/
│   ├── Program.cs              # Application entry point
│   ├── Endpoints/              # API endpoint definitions
│   │   ├── HealthEndpoints.cs
│   │   └── IngestEndpoints.cs
│   ├── Models/                 # Request/response models
│   │   ├── Event.cs
│   │   ├── EventLevel.cs
│   │   ├── HealthResponse.cs
│   │   └── IngestResponse.cs
│   └── Services/               # Business logic
│       └── EventStorageService.cs
└── MiniSOC.Server.Tests/       # Integration tests
    ├── HealthEndpointTests.cs
    └── IngestEndpointTests.cs
```

## Technology Stack
- ASP.NET Core 8 (Minimal APIs)
- xUnit (Testing)
- Swagger/OpenAPI (Documentation)

## Event Schema
Events follow schema v0.1 as defined in `docs/event-schema-v0.1.md`.

## Storage
Currently uses in-memory storage (ConcurrentDictionary). Events are lost on server restart.
Persistence layer will be added in a future milestone.

## Next Steps
See `docs/backlog.md` for planned features.