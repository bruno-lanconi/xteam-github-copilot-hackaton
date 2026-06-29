# Research: Book + Author Management Extension

## Decision 1: Keep in-memory repositories

- Decision: Implement author storage with `Dictionary<long, Author>` and keep books in existing in-memory collection.
- Rationale: Existing application uses in-memory model, scope asks standalone executable behavior, and fallback lookup needs fast id resolution.
- Alternatives considered:
  - Database persistence: rejected due scope and extra setup complexity.
  - File-based persistence: rejected because not required and increases failure modes.

## Decision 2: Strict `YYYY-MM-DD` date validation

- Decision: Validate `born_date` using exact date parsing (`DateTime.TryParseExact`) with invariant culture and strict format.
- Rationale: Requirement explicitly demands strict `YYYY-MM-DD`, and exact parsing prevents lenient acceptance.
- Alternatives considered:
  - Regex-only validation: rejected because format can pass while date invalid (example: `2026-02-31`).
  - General date parsing: rejected because non-strict formats may be accepted unexpectedly.

## Decision 3: Command parser upgrade for quoted arguments

- Decision: Replace naive split-based argument parsing for relevant commands with quoted token parsing.
- Rationale: Names and descriptions may contain spaces; existing parser breaks these inputs.
- Alternatives considered:
  - Keep whitespace split: rejected because breaks common names and descriptions.
  - Introduce JSON input mode: rejected as overkill for current CLI scope.

## Decision 4: Fallback author lifecycle

- Decision: Seed fallback author once at startup with `id=0`, `name="Unknown Author"`, and resolve missing author references to id `0`.
- Rationale: Requirement says auto-assign/create default author when author does not exist.
- Alternatives considered:
  - Reject invalid author id: rejected because conflicts with fallback requirement.
  - Create new fallback each time: rejected to avoid duplicate semantics and unstable behavior.

## Decision 5: CLI contract for author integration

- Decision: Use command contract:
  - `create_author <name> <born_date> [awards]`
  - `show_authors`
  - `add <title> <author_id> <category> <description>`
- Rationale: Directly supports id-based reference integrity and preserves existing command style.
- Alternatives considered:
  - Keep `add` with author name: rejected because requirement mandates `author_id` reference.
  - Separate `create_book` command: rejected to minimize churn in existing UX.
