# Tasks: Book + Author Management Extension

**Input**: Design documents from /specs/001-book-management/

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/cli-commands.md

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare command and test baseline for safe incremental delivery

- [ ] T001 Capture current CLI behavior expectations and update test scaffolding notes in Bookstore.Tests/UnitTest1.cs
- [ ] T002 Update command documentation comments for new command grammar in Bookstore/Program.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core model and parsing infrastructure required by all stories

**⚠️ CRITICAL**: No user story work starts before this phase completes

- [ ] T003 Create Author entity model with Id, Name, BornDate, AwardsList in Bookstore/Author.cs
- [ ] T004 Update Book entity to replace Author with AuthorId in Bookstore/Book.cs
- [ ] T005 Implement in-memory author repository and fallback seed (id 0, Unknown Author) in Bookstore/Program.cs
- [ ] T006 Implement strict born_date validator for YYYY-MM-DD in Bookstore/Program.cs
- [ ] T007 Implement quoted-argument tokenizer and safe command arg guards in Bookstore/Program.cs

**Checkpoint**: Foundation ready for story implementation

---

## Phase 3: User Story 1 - Create and List Authors (Priority: P1) 🎯 MVP

**Goal**: Create authors with strict date validation and display registered authors

**Independent Test**: Run create_author with valid and invalid dates, then run show_authors and verify output

### Tests for User Story 1

- [ ] T008 [US1] Add create_author success and invalid-date rejection tests in Bookstore.Tests/UnitTest1.cs
- [ ] T009 [US1] Add show_authors listing test in Bookstore.Tests/UnitTest1.cs

### Implementation for User Story 1

- [ ] T010 [US1] Add create_author command routing and handler in Bookstore/Program.cs
- [ ] T011 [US1] Add show_authors command routing and handler in Bookstore/Program.cs
- [ ] T012 [US1] Update help output to document create_author and show_authors usage in Bookstore/Program.cs

**Checkpoint**: User Story 1 independently functional and testable

---

## Phase 4: User Story 2 - Create Books Referencing Author ID (Priority: P1)

**Goal**: Add books using author_id reference and keep existing category/title validation behavior

**Independent Test**: Create one author, add one book with valid author id, run show, verify joined author name output path is used

### Tests for User Story 2

- [ ] T013 [US2] Update add command tests for author_id input contract in Bookstore.Tests/UnitTest1.cs
- [ ] T014 [US2] Add add-with-valid-author-id behavior test in Bookstore.Tests/UnitTest1.cs

### Implementation for User Story 2

- [ ] T015 [US2] Refactor add command parsing to accept title author_id category description in Bookstore/Program.cs
- [ ] T016 [US2] Update AddBook implementation to persist Book.AuthorId and resolve author name for success output in Bookstore/Program.cs
- [ ] T017 [US2] Update discontinueBook output to resolve author name from AuthorId in Bookstore/Program.cs

**Checkpoint**: User Stories 1 and 2 functional and independently testable

---

## Phase 5: User Story 3 - Fallback Unknown Author (Priority: P2)

**Goal**: Automatically map invalid or missing author references to Unknown Author (id 0)

**Independent Test**: Add book with non-existing author id and verify Unknown Author in resulting output

### Tests for User Story 3

- [ ] T018 [US3] Add add-with-invalid-author-id fallback test in Bookstore.Tests/UnitTest1.cs
- [ ] T019 [US3] Add regression test for fallback author availability at startup in Bookstore.Tests/UnitTest1.cs

### Implementation for User Story 3

- [ ] T020 [US3] Implement ResolveAuthorIdOrFallback helper and integrate in add flow in Bookstore/Program.cs
- [ ] T021 [US3] Ensure fallback author record initialization is idempotent and always present in Bookstore/Program.cs

**Checkpoint**: User Stories 1-3 functional and independently testable

---

## Phase 6: User Story 4 - Show Command Displays Author Name (Priority: P2)

**Goal**: Show books with resolved author names instead of raw author ids

**Independent Test**: Create valid-author and fallback-author books, run show, verify both author names rendered

### Tests for User Story 4

- [ ] T022 [US4] Add show output test asserting resolved author names for valid and fallback books in Bookstore.Tests/UnitTest1.cs

### Implementation for User Story 4

- [ ] T023 [US4] Update Show loop to join Book.AuthorId to author repository and render author name in Bookstore/Program.cs
- [ ] T024 [US4] Harden show join path with defensive fallback when author id missing in Bookstore/Program.cs

**Checkpoint**: All user stories independently functional

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, docs, and command UX consistency

- [ ] T025 [P] Add standalone demonstration scenario to project documentation in README.md
- [ ] T026 Run full test suite and fix residual regressions in Bookstore.Tests/UnitTest1.cs
- [ ] T027 [P] Normalize command error messages for missing args and invalid formats in Bookstore/Program.cs

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1): No dependencies
- Foundational (Phase 2): Depends on Phase 1 and blocks all stories
- User Story phases (Phase 3-6): Depend on Phase 2
- Polish (Phase 7): Depends on selected user stories completion

### User Story Dependencies

- US1 (P1): Starts after Phase 2, no dependency on other stories
- US2 (P1): Starts after Phase 2, depends on shared parsing/model foundation
- US3 (P2): Depends on US2 add flow path existing
- US4 (P2): Depends on US2 book-author reference and US3 fallback behavior

### Within Each User Story

- Tests first, then implementation
- Command routing before command behavior details
- Story checkpoint validation before moving forward

### Parallel Opportunities

- T025 and T027 can run in parallel (different files)
- After Phase 2, US1 and US2 can be worked in parallel by separate developers

---

## Parallel Example: User Story 1

- [ ] T008 [US1] Add create_author success and invalid-date rejection tests in Bookstore.Tests/UnitTest1.cs
- [ ] T010 [US1] Add create_author command routing and handler in Bookstore/Program.cs

---

## Parallel Example: User Story 2

- [ ] T013 [US2] Update add command tests for author_id input contract in Bookstore.Tests/UnitTest1.cs
- [ ] T015 [US2] Refactor add command parsing to accept title author_id category description in Bookstore/Program.cs

---

## Parallel Example: User Story 3

- [ ] T018 [US3] Add add-with-invalid-author-id fallback test in Bookstore.Tests/UnitTest1.cs
- [ ] T020 [US3] Implement ResolveAuthorIdOrFallback helper and integrate in add flow in Bookstore/Program.cs

---

## Parallel Example: User Story 4

- [ ] T022 [US4] Add show output test asserting resolved author names for valid and fallback books in Bookstore.Tests/UnitTest1.cs
- [ ] T023 [US4] Update Show loop to join Book.AuthorId to author repository and render author name in Bookstore/Program.cs

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 and Phase 2
2. Complete Phase 3 (US1)
3. Validate US1 independently
4. Demo create/list authors

### Incremental Delivery

1. Deliver US1 (author management)
2. Deliver US2 (book uses author id)
3. Deliver US3 (fallback behavior)
4. Deliver US4 (show name resolution)
5. Run polish and final validation

### Parallel Team Strategy

1. Developer A: US1 and US3 path
2. Developer B: US2 and US4 path
3. Integrate and run full suite at Phase 7
