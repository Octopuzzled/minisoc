# MiniSOC

A small, portfolio-friendly log collection and analysis platform (SIEM-inspired).
Goal: collect system events from endpoints, send to a server, store, and visualize.

## Status

Milestones 1–6 complete. Currently in Milestone 7: Polish.

- ✅ Agent: Event collection pipeline with pluggable sources
- ✅ Server: REST API with ingestion, filtering and metrics endpoints
- ✅ Database: SQLite persistence
- ✅ Web UI: Dashboard with event list, filters and charts
- ✅ Milestone 7: Polish (known issue fixes, UI improvements, code cleanup, docs)
- ⏳ Milestone 8: Real Windows Event Log ingestion, config system, packaging

## Scope (MVP)
- Agent: reads Windows Event Logs and sends structured events
- Server: REST API to ingest events + persistence
- UI: dashboard with filters and charts

## Non-goals (for now)
- ML/AI detection
- Real-time streaming
- Cloud deployment
- Multi-OS agent support (Windows agent first)

## Architecture
Agent (Windows) → HTTP → Server API → SQLite → Web UI

See `docs/diagrams/architecture.md` for a detailed diagram.

## Tech Stack
- Agent: C# (.NET 8, Console Application)
- Server: ASP.NET Core 8 (Minimal APIs)
- Database: SQLite (Microsoft.Data.Sqlite)
- UI: Vanilla HTML/CSS/JS with Chart.js

## Repository Structure
- `agent/`   Windows agent (C#)
- `server/`  API + persistence
- `web/`     Dashboard UI
- `docs/`    Documentation
- `scripts/` Helper scripts

## Getting Started
See `docs/setup/dev-environment.md`.

```bash
# Terminal 1 - Start server
./scripts/start-server.sh

# Terminal 2 - Start agent (optional)
./scripts/start-agent.sh

# Open UI
# Open web/index.html with Live Server in VS Code
```

## Documentation
- Component READMEs: `server/README.md`
- Decisions: `docs/decisions/`
- Setup: `docs/setup/`
- Diagrams: `docs/diagrams/`
- API contract: `docs/api-contract-v0.1.md`
- Event schema: `docs/event-schema-v0.1.md`
- Backlog: `docs/backlog.md`

## License
MIT