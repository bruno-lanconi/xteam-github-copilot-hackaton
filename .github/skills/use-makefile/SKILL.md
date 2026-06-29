---
name: use-makefile
description: 'Use when user asks to run tests, static analysis, linting, or quality checks through Makefile and containers. Triggers: make, Makefile, containerized checks, test pipeline, lint pipeline, static analysis.'
argument-hint: 'Task to run with make (test|static-analysis|lint|analyze)'
---

# Use Makefile Workflow

Standardize project automation through `make` targets that call containerized checks.

## When to Use
- User asks to run project checks using Makefile.
- User asks to add or update automation commands for tests, linting, or static analysis.
- User wants container-based quality gates that are easy to run locally and in CI.

## Procedure
1. Discover current automation surface.
- Check whether `Makefile` exists.
- Check available targets in `Bookstore/Dockerfile` or project Dockerfile.

2. Decide target strategy.
- If `Makefile` is missing: create one with clear, focused targets.
- If `Makefile` exists: extend minimally; do not break existing targets.
- If Docker targets are missing: add dedicated build stages first (`test`, `static-analysis`, `lint`, `analyze`).

3. Implement or update targets.
- Add `make test` for unit tests.
- Add `make static-analysis` for formatting/static checks.
- Add `make lint` for lint checks.
- Add `make analyze` for full quality pipeline.
- Keep variables configurable (`DOCKER`, `DOCKERFILE`, `BUILD_CONFIGURATION`).

4. Validate commands before claiming success.
- Run `make -n <targets>` to validate syntax and command wiring quickly.
- If requested, run full commands (`make test`, `make static-analysis`, `make lint`, `make analyze`).

5. Report outcomes.
- List changed files.
- Explain each target and which Docker build stage it invokes.
- Provide exact commands user can run next.

## Decision Points
- Need only one check type:
Use specific target (`test`, `static-analysis`, or `lint`).

- Need full gate:
Use `analyze` target that runs complete pipeline.

- Duplicate behavior between `static-analysis` and `lint`:
Split Docker stages so each target has distinct intent.

## Completion Criteria
- `Makefile` exists and contains `test`, `static-analysis`, `lint`, `analyze`.
- Each target maps to correct container stage.
- Dry-run (`make -n`) succeeds for all targets.
- Final runtime image path remains unaffected by quality-only stages.

## Output Contract
When this skill is used, produce:
- Minimal diffs.
- Verified make commands.
- A concise summary of target-to-container mapping.
