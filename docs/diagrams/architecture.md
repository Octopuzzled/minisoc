# Architecture (MVP)

```mermaid
flowchart LR
  subgraph Endpoint
    A[Windows Agent]
  end

  subgraph Backend
    S[Ingest API]
    D[(Database)]
    Q[Query API]
  end

  subgraph Frontend
    W[Web UI]
  end

  A -->|HTTP POST /ingest| S
  S -->|store| D
  Q -->|read| D
  W -->|HTTP GET /events /metrics| Q
