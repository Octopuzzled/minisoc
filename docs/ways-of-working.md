# Ways of Working

## Definition of Done (DoD) for a ticket

A task is "Done" when:

- Code builds/runs (if code changes exist)
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

## Issue structure (from Milestone 5)

Use the following template for feature issues:

**Problem**
What problem does this solve?

**Proposal**
What should be built? Include response structure / examples where relevant.

**Acceptance criteria**

- [ ] Testable, specific criteria

**Notes**
Links to ADRs, design decisions, dependencies on other issues.
