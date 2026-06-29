using Tasks;

namespace Bookstore;

public sealed class Bookstore
{
    private const string Quit = "quit";
    private const long UnknownAuthorId = 0;
    private const string UnknownAuthorName = "Unknown Author";
    private readonly IConsole console;

    private readonly IList<Book> books = new List<Book>();
    private readonly IDictionary<long, Author> authors = new Dictionary<long, Author>();
    private readonly IReadOnlyDictionary<int, string> categoryDictionary = new Dictionary<int, string>
    {
        { 1, "Fiction" },
        { 2, "Romance" },
        { 3, "Science Fiction" },
        { 4, "Fantasy" },
        { 5, "Mystery" },
        { 6, "Biography" },
    };

    private readonly IReadOnlyDictionary<string, int> categoryLookup;
    private long lastBookId;
    private long lastAuthorId;

    public static void Main(string[] args)
    {
        new Bookstore(new RealConsole()).Run();
    }

    public Bookstore(IConsole console)
    {
        this.console = console;
        categoryLookup = BuildCategoryLookup();
        EnsureFallbackAuthor();
    }

    public void Run()
    {
        while (true)
        {
            try
            {
                console.Write("> ");
                var commandLine = console.ReadLine();

                if (commandLine == Quit)
                {
                    break;
                }

                Execute(commandLine);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error:{0}", e.Message);
            }
        }
    }

    private void Execute(string commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            return;
        }

        var commandRest = commandLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var command = commandRest[0];

        switch (command)
        {
            case "show":
                Show();
                break;
            case "show_authors":
                ShowAuthors();
                break;
            case "add":
                if (commandRest.Length < 2)
                {
                    Console.Error.WriteLine("Missing arguments for add. Usage: add <title> <author_id> <category> <description>");
                    return;
                }

                Add(commandRest[1]);
                break;
            case "discontinueBook":
                if (commandRest.Length < 2)
                {
                    Console.Error.WriteLine("Missing arguments for discontinueBook. Usage: discontinueBook <book-id>");
                    return;
                }

                DiscontinueBook(commandRest[1]);
                break;
            case "discontinueAuthor":
                if (commandRest.Length < 2)
                {
                    Console.Error.WriteLine("Missing arguments for discontinueAuthor. Usage: discontinueAuthor <author-id>");
                    return;
                }

                DiscontinueByAuthor(commandRest[1]);
                break;
            case "create_author":
                if (commandRest.Length < 2)
                {
                    Console.Error.WriteLine("Missing arguments for create_author. Usage: create_author <name> <born_date> [awards]");
                    return;
                }

                CreateAuthor(commandRest[1]);
                break;
            case "help":
                Help();
                break;
            default:
                Error(command);
                break;
        }
    }

    private void Show()
    {
        if (books.Count == 0)
        {
            Console.Out.WriteLine("No books in the store.");
            return;
        }

        foreach (var book in books)
        {
            var status = book.IsDiscontinued ? "(discontinued)" : string.Empty;
            var authorName = ResolveAuthorName(book.AuthorId);
            Console.Out.WriteLine("[{0}] {1}: {2} {3}", book.Id, book.Title, authorName, status);
        }

        Console.Out.WriteLine();
    }

    private void ShowAuthors()
    {
        foreach (var author in authors.OrderBy(a => a.Key).Select(a => a.Value))
        {
            var awards = author.AwardsList.Count == 0 ? "(none)" : string.Join(", ", author.AwardsList);
            Console.Out.WriteLine("[{0}] {1} | born: {2} | awards: {3}", author.Id, author.Name, author.BornDate, awards);
        }

        Console.Out.WriteLine();
    }

    private void Add(string commandLine)
    {
        var args = TokenizeArguments(commandLine);

        if (args.Count < 4)
        {
            Console.Error.WriteLine("Invalid add arguments. Usage: add <title> <author_id> <category> <description>");
            return;
        }

        var title = args[0];
        var authorIdArg = args[1];
        var category = args[2];
        var description = string.Join(' ', args.Skip(3));

        long? authorId = null;
        if (long.TryParse(authorIdArg, out var parsedAuthorId))
        {
            authorId = parsedAuthorId;
        }

        AddBook(title, authorId, category, description);
    }

    private void CreateAuthor(string commandLine)
    {
        var args = TokenizeArguments(commandLine);

        if (args.Count < 2)
        {
            Console.Error.WriteLine("Invalid create_author arguments. Usage: create_author <name> <born_date> [awards]");
            return;
        }

        var name = args[0].Trim();
        var bornDate = args[1].Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.Error.WriteLine("Invalid author name.");
            return;
        }

        if (!IsStrictDate(bornDate))
        {
            Console.Error.WriteLine("Invalid born_date \"{0}\". Use YYYY-MM-DD.", bornDate);
            return;
        }

        var awards = new List<string>();
        if (args.Count > 2)
        {
            awards = string
                .Join(' ', args.Skip(2))
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList();
        }

        var author = new Author
        {
            Id = NextAuthorId(),
            Name = name,
            BornDate = bornDate,
            AwardsList = awards,
        };

        authors[author.Id] = author;
        Console.Out.WriteLine("Author created: [{0}] {1}", author.Id, author.Name);
    }

    private void AddBook(string title, long? authorId, string category, string description)
    {
        if (books.Any(b => string.Equals(b.Title, title, StringComparison.OrdinalIgnoreCase)))
        {
            Console.Out.WriteLine("Duplicate book with the name \"{0}\".", title);
            return;
        }

        if (!categoryLookup.TryGetValue(category, out var categoryId))
        {
            Console.Error.WriteLine("Invalid category \"{0}\".", category);
            return;
        }

        books.Add(
            new Book
            {
                Id = NextBookId(),
                Title = title,
                CategoryId = categoryId,
                Description = description,
                AuthorId = ResolveAuthorIdOrFallback(authorId),
                IsDiscontinued = false,
            }
        );

        var addedBook = books[^1];
        Console.Out.WriteLine("Book added: \"{0}\" by {1}", title, ResolveAuthorName(addedBook.AuthorId));
    }

    private void DiscontinueBook(string idString)
    {
        if (!long.TryParse(idString, out var id))
        {
            Console.Error.WriteLine("Invalid book id \"{0}\".", idString);
            return;
        }

        var book = books.FirstOrDefault(b => b.Id == id);

        if (book is null)
        {
            Console.Error.WriteLine("Book with ID {0} was not found.", id);
            return;
        }

        book.IsDiscontinued = true;
        Console.Out.WriteLine("Discontinued book: \"{0}\" by {1} (ID: {2})", book.Title, ResolveAuthorName(book.AuthorId), book.Id);
    }

    public void DiscontinueByAuthor(string authorIdString)
    {
        if (!long.TryParse(authorIdString, out var authorId))
        {
            Console.Error.WriteLine("Invalid author id \"{0}\".", authorIdString);
            return;
        }

        var matchedBooks = books.Where(b => b.AuthorId == authorId).ToList();

        if (matchedBooks.Count == 0)
        {
            Console.Out.WriteLine("No books found for author ID: {0}", authorId);
            return;
        }

        foreach (var matchedBook in matchedBooks)
        {
            matchedBook.IsDiscontinued = true;
        }

        Console.Out.WriteLine("Discontinued {0} book(s) for author {1}.", matchedBooks.Count, ResolveAuthorName(authorId));
    }

    private void Help()
    {
        Console.Out.WriteLine("=== Bookstore Commands ===");
        Console.Out.WriteLine();
        Console.Out.WriteLine("Commands:");
        Console.Out.WriteLine("  show");
        Console.Out.WriteLine("  show_authors");
        Console.Out.WriteLine("  create_author <name> <born_date> [awards]");
        Console.Out.WriteLine("  add <title> <author_id> <category> <description>");
        Console.Out.WriteLine("  discontinueBook <book-id>");
        Console.Out.WriteLine("  discontinueAuthor <author-id>");
        Console.Out.WriteLine("  help");
        Console.Out.WriteLine("  quit");
        Console.Out.WriteLine();
        Console.Out.WriteLine("Tip: use double quotes around values with spaces.");
        Console.Out.WriteLine();
        Console.Out.WriteLine("Available categories:");

        foreach (var category in categoryDictionary.Values)
        {
            Console.Out.WriteLine("  - {0}", category);
        }

        Console.Out.WriteLine();
    }

    private void Error(string command)
    {
        Console.Out.WriteLine("I don't know what the command \"{0}\" is.", command);
    }

    private IReadOnlyDictionary<string, int> BuildCategoryLookup()
    {
        var lookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in categoryDictionary)
        {
            lookup[kvp.Value] = kvp.Key;
        }

        return lookup;
    }

    private static bool IsStrictDate(string value)
    {
        return DateTime.TryParseExact(
            value,
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _
        );
    }

    private static List<string> TokenizeArguments(string input)
    {
        var tokens = new List<string>();
        var buffer = new System.Text.StringBuilder();
        var inQuotes = false;

        foreach (var ch in input)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(ch) && !inQuotes)
            {
                if (buffer.Length > 0)
                {
                    tokens.Add(buffer.ToString());
                    buffer.Clear();
                }

                continue;
            }

            buffer.Append(ch);
        }

        if (buffer.Length > 0)
        {
            tokens.Add(buffer.ToString());
        }

        if (inQuotes)
        {
            throw new FormatException("Unterminated quoted argument.");
        }

        return tokens;
    }

    private void EnsureFallbackAuthor()
    {
        if (authors.ContainsKey(UnknownAuthorId))
        {
            return;
        }

        authors[UnknownAuthorId] = new Author
        {
            Id = UnknownAuthorId,
            Name = UnknownAuthorName,
            BornDate = "1900-01-01",
            AwardsList = new List<string>(),
        };
    }

    private long ResolveAuthorIdOrFallback(long? inputAuthorId)
    {
        if (inputAuthorId is not null && authors.ContainsKey(inputAuthorId.Value))
        {
            return inputAuthorId.Value;
        }

        return UnknownAuthorId;
    }

    private string ResolveAuthorName(long authorId)
    {
        EnsureFallbackAuthor();

        if (authors.TryGetValue(authorId, out var author))
        {
            return author.Name;
        }

        return authors[UnknownAuthorId].Name;
    }

    private long NextBookId()
    {
        return ++lastBookId;
    }

    private long NextAuthorId()
    {
        return ++lastAuthorId;
    }
}
