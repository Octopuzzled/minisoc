# MiniSOC

A small, portfolio-friendly log collection and analysis platform (SIEM-inspired).
Goal: collect system events from endpoints, send to a server, store, and visualize.

## Status
This project is under active development.

**Currently implemented:**
- ✅ Server: REST API with /health and /ingest endpoints (in-memory storage)
- ✅ Event schema v0.1 defined
- ✅ API contract v0.1 defined

**In progress:**
- ⏳ Agent: implementation pending (lost during system reinstall, will be rebuilt)

**Planned:**
- ⏳ Database persistence
- ⏳ Query endpoints
- ⏳ Web UI

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

## Tech (planned)
- Agent: C# (.NET)
- Server: TBD (will be decided in docs/decisions)
- Database: TBD
- UI: TBD

## Repository structure
- `agent/`   Windows agent (C#)
- `server/`  API + persistence
- `web/`     UI
- `docs/`    documentation
- `scripts/` helper scripts

## Getting started
See `docs/setup/dev-environment.md`.

## Documentation
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
