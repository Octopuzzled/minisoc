# MiniSOC Server

REST API for the MiniSOC log collection platform.

## Status
- ✅ Health check endpoint implemented
- ⏳ Event ingestion (planned)
- ⏳ Query endpoints (planned)

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
  "timestamp": "2026-02-02T15:30:00Z",
  "version": "0.1.0"
}
```

**Status Codes:**
- `200 OK` - Service is healthy

### Test the API

**Browser:**
- Swagger UI: `http://localhost:5152/swagger`
- Health endpoint: `http://localhost:5152/health`

**curl:**
```bash
curl http://localhost:5152/health
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
│   ├── Program.cs           # Application entry point
│   ├── Endpoints/           # API endpoint definitions
│   │   └── HealthEndpoints.cs
│   └── Models/              # Response/request models
│       └── HealthResponse.cs
└── MiniSOC.Server.Tests/    # Integration tests
    └── HealthEndpointTests.cs
```

## Technology Stack
- ASP.NET Core 8 (Minimal APIs)
- xUnit (Testing)
- Swagger/OpenAPI (Documentation)

## Next Steps
See `docs/backlog.md` for planned features.