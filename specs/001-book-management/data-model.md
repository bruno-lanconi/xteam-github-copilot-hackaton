# Data Model: Book + Author Management Extension

## Entity: Author

- Purpose: Represent author metadata used by books.
- Fields:
  - `Id` (long): Unique identifier. Special reserved value `0` for fallback unknown author.
  - `Name` (string): Display name.
  - `BornDate` (string): Birth date in strict `YYYY-MM-DD` format.
  - `AwardsList` (List<string>): Award names.
- Validation rules:
  - `BornDate` must pass exact date parse in `YYYY-MM-DD` format.
  - `Name` should be non-empty after trimming.
- State notes:
  - Fallback Author (`Id=0`, `Name="Unknown Author"`) seeded during app startup.

## Entity: Book

- Purpose: Represent book catalog entry.
- Fields:
  - `Id` (long): Unique identifier.
  - `Title` (string): Book title.
  - `AuthorId` (long): Reference to `Author.Id`.
  - `CategoryId` (int): Category lookup id.
  - `Description` (string): Brief book description.
  - `IsDiscontinued` (bool): Discontinued flag.
- Validation rules:
  - `Title` must stay unique in current catalog behavior.
  - `CategoryId` must map to known category.
  - `AuthorId` must resolve to known author or fallback to `0`.

## Relationships

- Author (1) -> (N) Book by `Book.AuthorId`.
- Fallback behavior guarantees referential resolution even when requested author id does not exist.

## State Transitions

- Author:
  - `create_author` creates active author record.
  - No delete/discontinue flow in current scope.
- Book:
  - `add` creates active book.
  - `discontinueBook` sets `IsDiscontinued=true`.
  - `show` reads active/discontinued books and resolves author display name.
