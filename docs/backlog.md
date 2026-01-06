# Backlog (high level)

## Milestone 1: Architecture & interfaces
- [ ] Define event JSON schema (minimal)
- [ ] Define agent -> server API contract (endpoint, auth later)
- [ ] Draw a simple architecture diagram (text or mermaid)

## Milestone 2: Agent reads logs (local)
- [ ] Prototype: read Windows Event Logs (on Windows later)
- [ ] Implement agent core pipeline (collect -> normalize -> output JSON)

## Milestone 3: Transport
- [ ] Server skeleton (health endpoint)
- [ ] Ingest endpoint accepts JSON events
- [ ] Agent sends events via HTTP POST

## Milestone 4: Persistence
- [ ] Choose DB + schema
- [ ] Store events
- [ ] Query events by time + severity

## Milestone 5: Filters & metrics
- [ ] API filters
- [ ] Basic metrics endpoint(s)

## Milestone 6: UI
- [ ] List view + filters
- [ ] Simple charts

## Milestone 7: Polish
- [ ] Config system
- [ ] Packaging (later)

