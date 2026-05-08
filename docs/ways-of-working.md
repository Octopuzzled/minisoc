# Ways of Working

## Definition of Done (DoD) for a ticket

A task is "Done" when:

- Code builds/runs (if code changes exist)
- Unit tests added and passing (see `docs/testing.md`)
- Basic manual test performed and noted in the ticket/commit message
- Docs updated if behavior/usage changed
- No secrets committed
- Small, descriptive commit message

## Commit message style

Use prefixes like:

- feat: new feature
- fix: bug fix
- docs: documentation only
- chore: tooling/structure
- refactor: code restructure without behavior change
- test: test additions or modifications

## Branch naming
Use prefixes matching the commit type:
- `feat/` — new feature
- `fix/` — bug fix
- `docs/` — documentation only
- `chore/` — tooling/structure/cleanup
- `refactor/` — code restructure without behavior change
- `test/` — test additions

Examples: `feat/provider-filter`, `fix/known-issues`, `docs/adr-0005`, `test/agent-sources`

## Issue structure (from Milestone 5)

Use the following template for feature issues:

**Problem**
What problem does this solve?

**Proposal**
What should be built? Include response structure / examples where relevant.

**Acceptance criteria**

- [ ] Testable, specific criteria
- [ ] Tests added or updated

**Notes**
Links to ADRs, design decisions, dependencies on other issues.