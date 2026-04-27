```markdown
# MiniSOC Server

REST API for the MiniSOC log collection platform.

## Status
- ✅ Health check endpoint
- ✅ Event ingestion endpoint
- ✅ SQLite database setup and schema creation
- ✅ Event persistence (INSERT to database)
- ✅ Event retrieval with filters (level, host, provider, time range)
- ✅ Metrics endpoint (event count, breakdown by level and host, trend data)
- ✅ Web UI with dashboard, filters and charts

## Quick Start

### Run the server
```bash
cd server/MiniSOC.Server
dotnet run
```

Server starts on: `http://localhost:5152`

### Open the UI
Open `web/index.html` with Live Server in VS Code.

---

## Available Endpoints

### GET /health
Health check endpoint.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-03T15:30:00Z",
  "version": "0.1.0"
}
```

---

### POST /ingest
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

---

### GET /events
Query stored events with optional filters.

**Query Parameters (all optional):**
| Parameter | Type | Description |
|-----------|------|-------------|
| `level` | string | Filter by level (Error, Warning, Information) |
| `host` | string | Filter by host name |
| `provider` | string | Filter by provider (case-insensitive) |
| `startTime` | string | ISO 8601 timestamp, inclusive |
| `endTime` | string | ISO 8601 timestamp, inclusive |

**Response:** Array of event objects (empty array if no matches)

**curl example:**
```bash
curl "http://localhost:5152/events?level=Error&provider=Service%20Control%20Manager"
```

---

### GET /metrics
Returns aggregated event statistics.

**Response:**
```json
{
  "event_count": 135,
  "by_level": {
    "Error": 10,
    "Warning": 25,
    "Information": 100
  },
  "by_host": {
    "DESKTOP-1234": 80,
    "DESKTOP-5678": 55
  },
  "trend": {
    "last_24h": [
      { "time": "2026-04-17T13:00:00Z", "count": 12 }
    ],
    "last_7d": [
      { "time": "2026-04-11T00:00:00Z", "count": 85 }
    ]
  }
}
```

---

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
│   ├── Program.cs                    # Application entry point & DI configuration
│   ├── appsettings.json              # Configuration (connection strings)
│   ├── Endpoints/                    # API endpoint definitions
│   │   ├── HealthEndpoints.cs
│   │   ├── IngestEndpoints.cs
│   │   ├── EventsEndpoints.cs
│   │   └── MetricsEndpoints.cs
│   ├── Models/                       # Request/response models
│   │   ├── Event.cs
│   │   ├── EventLevel.cs
│   │   ├── HealthResponse.cs
│   │   ├── IngestResponse.cs
│   │   └── TrendBucket.cs
│   └── Services/                     # Business logic & data access
│       ├── IDatabaseService.cs       # Database interface
│       ├── SqliteDatabaseService.cs  # SQLite implementation
│       ├── IMetricsService.cs        # Metrics interface
│       └── SqliteMetricsService.cs   # SQLite metrics implementation
└── MiniSOC.Server.Tests/
    ├── DatabaseServiceTests.cs
    ├── EventPersistenceTests.cs
    ├── EventRetrievalTests.cs
    ├── HealthEndpointTests.cs
    ├── IngestEndpointTests.cs
    └── MetricsTests.cs
```

## Technology Stack
- ASP.NET Core 8 (Minimal APIs)
- SQLite with Microsoft.Data.Sqlite
- xUnit (Testing)
- Swagger/OpenAPI (Documentation)

## Event Schema
Events follow schema v0.1 as defined in `docs/event-schema-v0.1.md`.

## Next Steps
See `docs/backlog.md` for planned features (Milestone 7: Polish, Milestone 8: Agent & Real Events).
```