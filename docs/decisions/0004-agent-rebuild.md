# ADR 0004: Agent rebuild after system reinstall

Date: 2026-02-09
Status: Accepted

## Context
Agent code was lost during a system reinstall. 
The server (from Milestone 3) already exists and works well.
We need to rebuild the agent (Milestone 2).

## Decision
Rebuild the agent from scratch using a clean interface-based architecture:
- `IEventSource` interface for pluggable event collection
- `DummyEventSource` implementation for Linux development
- `WindowsEventLogSource` implementation deferred to Windows development phase

## Rationale
- Lost code was an early prototype without clean architecture
- Rebuilding allows us to implement proper separation of concerns from the start
- Interface-based design enables Linux-first development while preparing for Windows support
- Agent-Server contract (API v0.1, Schema v0.1) is already defined and tested

## Consequences
+ Clean architecture from the start (not a retrofit)
+ OS-agnostic development workflow
+ Better testability
- Short delay in agent availability (~1-2 hours rebuild time)

## Implementation Plan
1. Create `agent/` directory structure
2. Implement `IEventSource` interface
3. Implement `DummyEventSource` for development
4. Implement HTTP client for /ingest endpoint
5. Add configuration system
6. Document in `docs/setup/agent.md`