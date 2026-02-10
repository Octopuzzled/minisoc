# MiniSOC

A small, portfolio-friendly log collection and analysis platform (SIEM-inspired).
Goal: collect system events from endpoints, send to a server, store, and visualize.

## Status
MVP complete! Agent collects and sends events to server via HTTP.

**Currently implemented:**
- ✅ Agent: Event collection with pluggable sources (DummyEventSource for development)
- ✅ Agent: HTTP transmission to server (/ingest endpoint)
- ✅ Server: REST API with /health and /ingest endpoints (in-memory storage)
- ✅ Event schema v0.1 defined and implemented
- ✅ API contract v0.1 defined and implemented
- ✅ Helper scripts for easy development

**Next up (Milestone 4):**
- ⏳ Database persistence (SQLite or PostgreSQL)
- ⏳ Query endpoints (filter by time, severity, host)

**Planned:**
- ⏳ Advanced filters & metrics endpoints (Milestone 5)
- ⏳ Windows Event Log integration (Milestone 2 - Windows phase)
- ⏳ Web UI (Milestone 6)

## Scope (MVP)
- Agent: reads Windows Event Logs and sends structured events
- Server: REST API to ingest events + persistence
- UI: basic dashboards + filters

## Non-goals (for now)
- ML/AI detection
- Real-time streaming
- Cloud deployment
- Multi-OS agent support (Windows agent first)

## Architecture (high level)
Agent (Windows) -> HTTP -> Server API -> Database -> Web UI

## Tech Stack
- Agent: C# (.NET 8, Console Application)
- Server: ASP.NET Core 8 (Minimal APIs)
- Database: TBD (Milestone 4)
- UI: TBD (Milestone 6)

## Repository structure
- `agent/`   Windows agent (C#)
- `server/`  API + persistence
- `web/`     UI
- `docs/`    documentation
- `scripts/` helper scripts

## Getting started
See `docs/setup/dev-environment.md`.

**Quick start:**
```bash
# Terminal 1 - Start server
./scripts/start-server.sh

# Terminal 2 - Start agent
./scripts/start-agent.sh
```

## Documentation
- Component READMEs: `agent/README.md`, `server/README.md`
- Decisions: `docs/decisions/`
- Setup: `docs/setup/`
- Diagrams: `docs/diagrams/`

## Roadmap
- Milestones and high-level backlog: `docs/backlog.md`
- Architecture diagram: `docs/diagrams/architecture.md`
- Event schema: `docs/event-schema-v0.1.md`
- API contract: `docs/api-contract-v0.1.md`

## License
TBD