# API Contract v0.1

## Health check
**GET** `/health`

### Response
- `200 OK` — server is running

---

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
```

### Response
- `200 OK` — events stored successfully
- `400 Bad Request` — invalid event format

---

## Query events
**GET** `/events`

### Query parameters (all optional)
| Parameter | Type | Description |
|-----------|------|-------------|
| `level` | string | Filter by level (Error, Warning, Information) |
| `host` | string | Filter by host name |
| `provider` | string | Filter by provider (case-insensitive) |
| `startTime` | string | ISO 8601 timestamp, inclusive |
| `endTime` | string | ISO 8601 timestamp, inclusive |

### Response
- `200 OK` — array of event objects (empty array if no matches)

#### Example
```json
[
  {
    "event_id": "8d9b1e2d-...",
    "timestamp": "2026-01-12T18:00:00Z",
    "host": "DESKTOP-1234",
    "source": "WindowsEventLog",
    "channel": "System",
    "provider": "Service Control Manager",
    "level": "Error",
    "message": "The service XYZ failed to start.",
    "raw": null
  }
]
```

---

## Metrics
**GET** `/metrics`

### Response
- `200 OK` — aggregated event statistics

#### Example
```json
{
  "event_count": 135,
  "by_level": {
    "Error": 10,
    "Warning": 25,
    "Information": 100
  },
  "by_host": {
    "DESKTOP-1234": 80,
    "DESKTOP-5678": 55
  },
  "trend": {
    "last_24h": [
      { "time": "2026-04-17T13:00:00Z", "count": 12 }
    ],
    "last_7d": [
      { "time": "2026-04-11T00:00:00Z", "count": 85 }
    ]
  }
}
```