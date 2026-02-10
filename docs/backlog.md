# Backlog (high level)

## Milestone 1: Architecture & interfaces
- [x] Define event JSON schema (minimal)  ← checkboxen aktualisieren
- [x] Define agent -> server API contract
- [x] Draw architecture diagram

## Milestone 2: Agent reads logs (local)
- [x] Prototype: DummyEventSource (development)
- [x] Implement agent core pipeline (collect -> normalize -> output JSON)
- [ ] Read Windows Event Logs (deferred to Windows phase)

## Milestone 3: Transport
- [x] Server skeleton (health endpoint)
- [x] Ingest endpoint accepts JSON events
- [x] Agent sends events via HTTP POST

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

