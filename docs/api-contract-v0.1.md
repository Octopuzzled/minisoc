# API Contract v0.1

## Ingest events
**POST** `/ingest`

### Request body
- Content-Type: application/json
- Either a single event object or an array of events

#### Example (array)
```json
[
  {
    "timestamp": "2026-01-12T18:00:00Z",
    "host": "DESKTOP-1234",
    "source": "WindowsEventLog",
    "channel": "System",
    "provider": "Service Control Manager",
    "level": "Error",
    "message": "The service XYZ failed to start."
  }
]
