# Backlog (high level)

## Milestone 1: Architecture & interfaces ✅
- [x] Define event JSON schema (minimal)
- [x] Define agent -> server API contract
- [x] Draw architecture diagram

## Milestone 2: Agent reads logs ✅
- [x] Prototype: DummyEventSource (development)
- [x] Implement agent core pipeline (collect -> normalize -> output JSON)

## Milestone 3: Transport ✅
- [x] Server skeleton (health endpoint)
- [x] Ingest endpoint accepts JSON events
- [x] Agent sends events via HTTP POST

## Milestone 4: Persistence ✅
- [x] Choose DB + schema (SQLite, ADR 0005)
- [x] Store events
- [x] Query events by time + severity

## Milestone 5: Filters & metrics ✅
- [x] provider filter on GET /events
- [x] GET /metrics endpoint (event count, by level, by host)
- [x] Trend data in GET /metrics (last 24h, last 7d)

## Milestone 6: UI ✅
- [x] Event list with dark theme
- [x] Filters (level, provider, time range)
- [x] Charts (by level, by host, trend)

## Milestone 7: Polish ✅
- [x] Fix known issues (timezone, case-insensitive provider)
- [x] UI improvements (header, metric cards, grid layout)
- [x] Code cleanup and comment review
- [x] Docs update

## Milestone 8: Agent & Real Events ✅
- [x] Real Windows Event Log ingestion
- [x] Config system
- [ ] Packaging