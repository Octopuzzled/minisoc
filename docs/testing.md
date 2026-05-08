# Testing Guide

## Overview

Tests are organized in the `tests/` folder at the repository root, separate from source code:
- `tests/MiniSOC.Agent.Tests/` — Agent unit and component tests
- `tests/MiniSOC.Server.Tests/` — Server integration and API tests

## Test Framework

- **Framework:** xUnit (.NET standard testing framework)
- **Assertion Library:** Fluent Assertions (readable test expressions)
- **Mocking:** Moq (for interface mocking)

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run tests for a specific project
```bash
dotnet test tests/MiniSOC.Agent.Tests
dotnet test tests/MiniSOC.Server.Tests
```

### Run tests with verbose output
```bash
dotnet test --verbosity detailed
```

## Test Structure
Each test project mirrors the source structure:

```
tests/MiniSOC.Agent.Tests/
├── DateTimeExtensionsTests.cs
├── MiniSOC.Agent.Tests.csproj
└── ...

tests/MiniSOC.Server.Tests/
├── HealthEndpointTests.cs
├── IngestEndpointTests.cs
├── EventPersistenceTests.cs
├── EventRetrievalTests.cs
├── DatabaseServiceTests.cs
├── MetricsTests.cs
├── GlobalUsings.cs
├── MiniSOC.Server.Tests.csproj
└── ...
```

## Definition of Done for Tests
When adding or modifying code:

✅ Unit tests cover happy path and error cases
✅ Integration tests verify component interaction
✅ All tests pass locally (dotnet test)
✅ Tests are named descriptively
✅ Test comments explain "why" not "what"

## Common Issues
**Q: Tests fail locally but pass in CI?**

- Ensure .NET 8 SDK is installed: dotnet --version
- Clean build: dotnet clean && dotnet build
- Check for hardcoded paths or environment assumptions

**Q: Should I test private methods?**

- No. Test public interfaces; private methods are tested indirectly.

**Q: Test database files not being deleted?**

- Ensure you call SqliteConnection.ClearAllPools() before File.Delete()
- This releases all connections so the file can be deleted