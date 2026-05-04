# Development Environment

## Development OS
- Server, UI: Linux (Ubuntu) or Windows
- Agent: Windows only (requires Windows Event Log access)

## Target runtime
- Agent runs on Windows (requires Administrator privileges)
- Server + UI: platform-independent

## Tools
- Git
- Editor: VS Code
- .NET 8 SDK
- Live Server (VS Code Extension, for UI development)
- SQLite (via Microsoft.Data.Sqlite, no separate tool needed)

## Notes
- Most components can be developed on Linux.
- Windows-specific parts are isolated in `agent/`.
- The agent requires Administrator privileges to read the Security event log channel.

## Quick Start

For detailed setup instructions, see:
- Server: `server/README.md`
- Agent: `agent/README.md`