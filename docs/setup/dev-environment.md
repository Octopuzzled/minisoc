# Development Environment

## Current development OS
- Linux (Ubuntu)

## Target runtime
- Agent runs on Windows
- Server + UI: platform-independent

## Tools
- Git
- Editor: VS Code
- .NET 8 SDK
- Live Server (VS Code Extension, für UI-Entwicklung)
- SQLite (via Microsoft.Data.Sqlite, kein separates Tool nötig)

## Notes
- We develop most components on Linux.
- Windows-specific parts are isolated in `agent/`.

## Quick Start

For detailed setup instructions, see:
- Server: `server/README.md`
- Agent: `agent/README.md`