# ADR 0002: Tech stack decisions (initial)

Date: 2026-01-12
Status: Accepted

## Context
We develop on Linux. Agent targets Windows.
We need a simple server + DB + UI to ingest and visualize events.

## Decision
For milestone 1 we keep server/db/ui as TBD.
We only define interfaces (schema + API contract) to avoid premature decisions.

## Next step
In milestone 3/4 we will choose:
- Server framework
- Database
- UI framework
based on simplicity and learning value.
