# ADR 0003: API architecture - Minimal APIs with structured organization

Date: 2026-02-02
Status: Accepted

## Context
We need to implement a REST API for the MiniSOC server (health check, event ingestion, queries).
ASP.NET Core offers two approaches:
- Controller-based (classic, more boilerplate)
- Minimal APIs (modern, less code)

We develop as a solo developer. The MVP scope includes ~4-6 endpoints.
Development happens on Linux; the server is platform-independent.

## Decision
We use **Minimal APIs** with structured organization via extension methods.

Instead of putting all routes in `Program.cs`, we organize endpoints in separate files:
- `Endpoints/HealthEndpoints.cs`
- `Endpoints/IngestEndpoints.cs`
- `Endpoints/QueryEndpoints.cs`

Each file contains an extension method like `MapHealthEndpoints(this WebApplication app)`.

## Rationale
- Solo project with <10 endpoints (no need for heavy controller scaffolding)
- Modern .NET standard (6+), shows up-to-date knowledge
- Less boilerplate while maintaining testability
- Faster iteration for MVP phase
- Refactoring to controllers possible in <1 day if needed (e.g., when scaling to 20+ endpoints)

## Consequences
- Program.cs stays clean and readable
- Endpoints are grouped by domain (health, ingest, query)
- Testing remains straightforward (WebApplicationFactory works with both approaches)
- If project grows significantly: consider refactoring to controllers
  - This is acceptable because unlikely for current MVP scope
  - Migration path is well-documented and non-breaking for clients

## Alternatives considered
- **Controller-based approach**: More structure upfront, but overkill for current scope
- **Everything in Program.cs**: Fast to start, but becomes unmaintainable quickly