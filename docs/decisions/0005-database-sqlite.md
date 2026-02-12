# ADR 0005: Database choice and schema (SQLite)

Date: 2026-02-12
Status: Accepted

## Context

To guarantee event persistence, we need to implement a database. This requires decisions about architecture and design.

## Decisions

**Database**

We choose SQLite for its simplicity and sufficient functionality.

**UUID instead of AUTOINCREMENT or Hash for event id**

- UUID will be generated before INSERT
- No problems with duplicates (agent doesn't try to resend if network error occurs)
- Works with multi agent/pc architecture
- KISS: easier than hash-based deduplication

**Package: Microsoft.Data.SQLite**

- Best for .NET 8
- No legacy needs to be supported -> no System.Data.SQLite
- No ORM needed for the scope of the project

## DB Schema

Table "Events":

- event_id TEXT NOT NULL PRIMARY KEY -- UUID as String
- timestamp TEXT NOT NULL
- host TEXT NOT NULL
- source TEXT NOT NULL
- channel TEXT
- provider TEXT
- level TEXT NOT NULL
- message TEXT

## Rationale

SQLite with Microsoft.Data.Sqlite provides sufficient persistence
for the MVP without introducing unnecessary complexity.

## Consequences

- Simple architecture
- No duplication issues
- Modern packages

## Implementation Plan

1. On server startup: establish database connection
2. On server startup: create "Events" table if it doesn't exist (migration)
3. Implement INSERT to store events (parameterized queries to prevent SQL injection)
4. Implement SELECT to retrieve events
5. Replace in-memory storage with SQLite service
