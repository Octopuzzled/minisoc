# Helper Scripts

Convenience scripts for running MiniSOC components during development.

## Available Scripts

### `start-server.sh`
Starts the MiniSOC server.

```bash
./scripts/start-server.sh
```

**What it does:**
- Navigates to `server/MiniSOC.Server`
- Runs `dotnet run`
- Server listens on `http://localhost:5152`

**Requirements:**
- .NET 8 SDK installed
- Run from repository root

---

### `start-agent.sh`
Starts the MiniSOC agent.

```bash
./scripts/start-agent.sh
```

**What it does:**
- Prompts to ensure server is running
- Navigates to `agent/MiniSOC.Agent`
- Runs `dotnet run`
- Sends 5 test events to server

**Requirements:**
- .NET 8 SDK installed
- Server running (start with `start-server.sh` first)
- Run from repository root

---

## Typical Workflow

**Terminal 1 (Server):**
```bash
./scripts/start-server.sh
# Leave running
```

**Terminal 2 (Agent):**
```bash
./scripts/start-agent.sh
# Runs once, then exits
```

---

## Making Scripts Executable

If you get "Permission denied":

```bash
chmod +x scripts/*.sh
```

---

## Troubleshooting

**Error: "dotnet command not found"**
- Install .NET 8 SDK: https://dotnet.microsoft.com/download

**Error: "directory not found"**
- Make sure you're in the repository root
- Run: `pwd` - should show path ending in `/minisoc`

**Agent: "Network error"**
- Make sure server is running first in another terminal
- Check server is on `http://localhost:5152`
