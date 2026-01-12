# Event Schema v0.1 (MVP)

## Goals
- Minimal fields for storing + filtering
- Works for Windows Event Logs first
- Extensible later

## JSON example

```json
{
  "event_id": "uuid-or-hash",
  "timestamp": "2026-01-12T18:00:00Z",
  "host": "DESKTOP-1234",
  "source": "WindowsEventLog",
  "channel": "System",
  "provider": "Service Control Manager",
  "level": "Error",
  "message": "The service XYZ failed to start.",
  "raw": {
    "eventRecordId": 12345,
    "keywords": ["Classic"]
  }
}
