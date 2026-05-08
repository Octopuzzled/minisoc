# MiniSOC

MiniSOC is a small SIEM-inspired log collection and analysis platform built as a portfolio project.

It collects Windows Event Logs from endpoints, forwards them to a central server, stores them in SQLite, and visualizes them in a web dashboard with filtering and metrics.

---

## Features

- Windows Event Log collection
- Structured event forwarding via HTTP
- REST API for ingestion and querying
- SQLite persistence
- Dashboard with:
  - Event table
  - Filtering
  - Metrics cards
  - Charts and trends
- Self-contained executable deployment

---

## Status

Milestones 1–8 complete. ✅

### Completed

- ✅ Agent: Real Windows Event Log ingestion with configurable channels and levels
- ✅ Server: REST API with ingestion, filtering, and metrics endpoints
- ✅ Database: SQLite persistence
- ✅ Web UI: Dashboard with filters, charts, and metrics
- ✅ Packaging: Self-contained executables for server and agent

---

## Architecture

```txt
Agent (Windows)
        ↓ HTTP
Server API (ASP.NET Core)
        ↓
SQLite Database
        ↓
Dashboard UI
````

See `docs/diagrams/architecture.md` for a detailed diagram.

---

## Tech Stack

### Agent

- C#
- .NET 8
- Windows Event Logs API
- Console Application

### Server

- ASP.NET Core 8 Minimal APIs
- SQLite
- REST API

### Frontend

- Vanilla HTML/CSS/JavaScript
- Chart.js

---

## Repository Structure

```txt
agent/       Windows event collection agent
server/      API server, persistence and dashboard
tests/       Unit and integration tests
docs/        Documentation and diagrams
scripts/     Helper scripts
```

---

## Downloads

Pre-built self-contained executables are available in the GitHub Releases section.

Download and extract:

- `MiniSOC-Server-win-x64.zip`
- `MiniSOC-Agent-win-x64.zip`

No .NET SDK or runtime installation is required.

---

## Running MiniSOC

### 1. Start the Server

Open the extracted server folder and run:

```powershell
MiniSOC.Server.exe
```

The dashboard will be available at:

```txt
http://localhost:5000
```

Keep the server running while using agents.

---

### 2. Start the Agent

Open the extracted agent folder and run:

```powershell
MiniSOC.Agent.exe
```

Administrator privileges may be required to access Windows Event Logs.

The agent will automatically collect Windows Event Logs and forward them to the server.

---

## Building from Source

### Server

```bash
dotnet publish -c Release --self-contained -r win-x64
```

Published files will be located in:

```txt
server/bin/Release/net8.0/win-x64/publish/
```

---

### Agent

```bash
dotnet publish -c Release --self-contained -r win-x64
```

Published files will be located in:

```txt
agent/bin/Release/net8.0/win-x64/publish/
```

---

## Development Setup

For local development setup instructions, see:

```txt
docs/setup/dev-environment.md
```

---

## Current Limitations

- Windows-only agent support
- No authentication or RBAC yet
- No alerting pipeline
- No real-time streaming
- SQLite only
- Intended as an educational/portfolio project, not production SIEM software

---

## Future Improvements

- Authentication and user management
- Detection rules and alerts
- Docker deployment
- Multi-agent support
- Linux log ingestion
- WebSocket live updates
- Elasticsearch/OpenSearch backend

---

## Notes

- The dashboard refreshes automatically every 30 seconds.
- The server must be running before agents can connect.
- The project is intentionally lightweight and focused on learning backend, systems, and security engineering concepts.
