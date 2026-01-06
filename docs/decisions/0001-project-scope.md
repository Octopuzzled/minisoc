# ADR 0001: Project scope and goals

Date: 2026-01-06
Status: Accepted

## Context
We want a SIEM-inspired portfolio project to learn professional development practices.
Development happens on Linux; the agent targets Windows.

## Decision
Build an MVP with:
- Windows agent: reads Windows Event Logs and sends structured events (JSON) to a server
- Server: ingest API + persistence
- Web UI: basic dashboards and filters

## Out of scope (for now)
- ML/AI detection
- Real-time streaming
- Cloud deployment
- Multi-OS agent support

## Consequences
- Windows-specific code is isolated in `agent/`
- Most development can proceed on Linux
