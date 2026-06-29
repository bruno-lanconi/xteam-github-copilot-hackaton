# Feature Specification: Book + Author Management Extension

**Feature Branch**: `001-book-management`

**Created**: 2026-06-29

**Status**: Draft

**Input**: User description: "Extend Book Management System with Author entity, author fallback, and updated show commands"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create and List Authors (Priority: P1)

As a bookstore operator, I create authors with validated birth dates and view registered authors.

**Why this priority**: Author record becomes source of truth for book attribution.

**Independent Test**: Run `create_author` with valid and invalid dates, then run `show_authors` and verify persisted output.

**Acceptance Scenarios**:

1. **Given** empty author repository except default unknown author, **When** operator runs `create_author "Frank Herbert" 1920-10-08 "Hugo;Nebula"`, **Then** system stores author and prints success with generated id.
2. **Given** operator provides malformed date, **When** `create_author` executes, **Then** system rejects input with validation error and does not store author.

---

### User Story 2 - Create Books Referencing Author ID (Priority: P1)

As a bookstore operator, I create books using author IDs so book records reference author entities.

**Why this priority**: Core integration requirement. Book must reference author entity, not plain name string.

**Independent Test**: Create one author, create one book using that author id, run `show`, verify author name join.

**Acceptance Scenarios**:

1. **Given** author id exists, **When** operator runs `add` with that id, **Then** book stores `author_id` and output displays resolved author name.

---

### User Story 3 - Fallback Unknown Author (Priority: P2)

As a bookstore operator, when I use missing or invalid author id, system auto-assigns default unknown author so command still succeeds.

**Why this priority**: Required resiliency behavior.

**Independent Test**: Create book with non-existing author id, run `show`, verify displayed author name is `Unknown Author` and fallback author id is `0`.

**Acceptance Scenarios**:

1. **Given** author id does not exist, **When** `add` runs, **Then** system maps to fallback author (`id=0`, `name="Unknown Author"`).

---

### User Story 4 - Show Command Displays Author Name (Priority: P2)

As a bookstore operator, I need book listing to show author names, not raw ids.

**Why this priority**: Usability and reporting clarity.

**Independent Test**: Create books with valid and fallback authors, run `show`, verify output includes author names for each record.

**Acceptance Scenarios**:

1. **Given** books with mixed author ids, **When** `show` runs, **Then** output resolves and prints author names consistently.

### Edge Cases

- `create_author` called with duplicate name and same birth date: accepted as separate record unless uniqueness rule introduced later.
- `create_author` called with date `2026-2-9`: rejected because format not strict `YYYY-MM-DD`.
- `add` called with missing `author_id`: command rejected with usage error.
- `show_authors` called when only fallback author exists: output still valid and indicates repository contents.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST define new `Author` entity with `id`, `name`, `born_date`, `awards_list`.
- **FR-002**: System MUST validate `born_date` in strict `YYYY-MM-DD` format during author creation.
- **FR-003**: System MUST support `create_author` command that persists author in in-memory repository.
- **FR-004**: System MUST support `show_authors` command that returns formatted list of all registered authors.
- **FR-005**: System MUST update `Book` entity to store `author_id` reference.
- **FR-006**: System MUST resolve book `author_id` to author name in `show` output.
- **FR-007**: System MUST apply fallback logic when provided `author_id` missing or invalid, assigning default author (`id=0`, `name="Unknown Author"`).
- **FR-008**: System MUST maintain standalone execution using in-memory storage only.
- **FR-009**: System MUST provide demonstration flow covering create author, add valid-author book, add fallback-author book, and show commands.

### Key Entities *(include if feature involves data)*

- **Author**: Book author record with identifier, display name, strict date string, and award names.
- **Book**: Book record containing metadata and `author_id` foreign-key style reference to `Author`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100 percent of `create_author` inputs with non-`YYYY-MM-DD` date are rejected in tests.
- **SC-002**: 100 percent of `show` outputs display author names (never raw author ids only).
- **SC-003**: 100 percent of book creations with missing/invalid author id resolve to fallback author id `0`.
- **SC-004**: End-to-end demonstration sequence executes without runtime exceptions.

## Assumptions

- Existing command-line app remains single-process and in-memory.
- Existing category behavior remains unchanged.
- No persistence/database work in this scope.
- Command parser enhancement for quoted arguments is in scope.
