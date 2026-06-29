# Quickstart: Validate Book + Author Management Extension

## Prerequisites

- .NET SDK compatible with solution
- Repository root as working directory

## 1. Run tests

```bash
dotnet test
```

Expected outcome:
- Test suite passes including new author creation, strict date validation, fallback author behavior, and show command name resolution.

## 2. Run application

```bash
dotnet run --project Bookstore
```

## 3. Execute validation scenario

Use command sequence below in interactive CLI:

```text
create_author "Frank Herbert" 1920-10-08 "Hugo;Nebula"
add "Dune" 1 Fiction "Classic sci-fi epic"
add "Mystery Book" 999 Fantasy "Unknown author fallback case"
show_authors
show
quit
```

Expected outcome:
- First command creates author and prints generated id.
- Second add uses valid author id and shows Frank Herbert as author.
- Third add uses missing author id and resolves to Unknown Author (`id=0`).
- `show_authors` lists known authors in formatted output.
- `show` displays author names, not only numeric author ids.

## 4. Validate date rejection

Run app again and execute:

```text
create_author "Bad Date" 1920/10/08 "None"
quit
```

Expected outcome:
- Command rejected with born date validation error.
- Invalid author not persisted.
