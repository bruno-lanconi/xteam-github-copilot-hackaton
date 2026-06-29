# Implementation Plan: Book + Author Management Extension

**Branch**: `001-book-management` | **Date**: 2026-06-29 | **Spec**: `/specs/001-book-management/spec.md`

**Input**: Feature specification from `/specs/001-book-management/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add first-class Author entity to existing C# CLI bookstore. Migrate Book to `AuthorId` reference. Add `create_author` and `show_authors` commands with strict born-date validation. Update `show` output to resolve and print author name. Enforce fallback author (`id=0`, `Unknown Author`) when book creation gets missing/invalid author id. Keep in-memory storage and standalone execution.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# (.NET 10 project SDK in repository)

**Primary Dependencies**: .NET runtime libraries, xUnit test framework

**Storage**: In-memory collections (`List<Book>`, `Dictionary<long, Author>`)

**Testing**: xUnit via `dotnet test`

**Target Platform**: Cross-platform CLI (macOS/Linux/Windows)

**Project Type**: CLI application + unit tests

**Performance Goals**: Interactive command latency under 100ms for in-memory operations on current expected dataset sizes

**Constraints**: No database; preserve existing command loop behavior; strict date validation `YYYY-MM-DD`; maintain backward compatibility where possible for non-author commands

**Scale/Scope**: Single-user CLI session; tens to low hundreds of book/author records in memory

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Constitution file currently template placeholders without enforceable project rules.

Pre-Phase-0 gate result: PASS with assumption.

- No concrete mandatory principles detected.
- Default engineering gates applied instead:
  - keep scope minimal and in-memory,
  - add/adjust tests for new behaviors,
  - avoid unrelated refactors.

Post-Phase-1 re-check: PASS.

- Planned artifacts (research, data model, contract, quickstart) align with requested workflow.
- No policy conflicts identified due placeholder constitution.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Bookstore/
├── Program.cs
├── Book.cs
├── Author.cs                # new
├── IConsole.cs
└── RealConsole.cs

Bookstore.Tests/
└── UnitTest1.cs

specs/001-book-management/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── cli-commands.md
```

**Structure Decision**: Keep existing single CLI project and test project. Add one new entity file and documentation artifacts under `specs/001-book-management`.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --------- | ---------- | ------------------------------------ |
| None      | N/A        | N/A                                  |
