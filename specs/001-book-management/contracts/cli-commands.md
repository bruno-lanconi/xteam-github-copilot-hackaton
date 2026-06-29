# Contract: CLI Commands for Book + Author Management

## Command: create_author

- Syntax: `create_author <name> <born_date> [awards]`
- Input rules:
  - `<name>` supports quoted text for spaces.
  - `<born_date>` must be strict `YYYY-MM-DD`.
  - `[awards]` optional. If provided, recommended semicolon-delimited in one quoted argument.
- Success behavior:
  - Persists author in memory.
  - Prints confirmation with generated author id.
- Failure behavior:
  - Prints validation error when date format invalid.
  - Command does not persist invalid author.

## Command: show_authors

- Syntax: `show_authors`
- Success behavior:
  - Prints formatted author list including id, name, born date, awards.
  - Includes fallback author if present in repository.

## Command: add

- Syntax: `add <title> <author_id> <category> <description>`
- Input rules:
  - `<author_id>` parsed as long.
  - `<category>` must map to known category list.
  - Quoted text supported for title/description.
- Success behavior:
  - Creates book record with resolved `AuthorId`.
  - If provided author id missing/invalid, uses fallback author id `0`.
  - Prints added confirmation with resolved author name.

## Command: show

- Syntax: `show`
- Success behavior:
  - Prints books with author name resolved from `AuthorId`.
  - Never prints raw unresolved author id in main book display line.

## Backward Compatibility Notes

- Existing commands (`help`, `quit`, `discontinueBook`) remain available.
- `discontinueAuthor` behavior should avoid parse exceptions under id-based author model.
