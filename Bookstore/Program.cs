using Tasks;

namespace Bookstore;

public sealed class Bookstore
{
    private const string Quit = "quit";
    private readonly IConsole console;

    private readonly IList<Book> books = new List<Book>();
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
    private long lastId;

    public static void Main(string[] args)
    {
        new Bookstore(new RealConsole()).Run();
    }

    public Bookstore(IConsole console)
    {
        this.console = console;
        categoryLookup = BuildCategoryLookup();
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

        var commandRest = commandLine.Split(' ', 2);
        var command = commandRest[0];

        switch (command)
        {
            case "show":
                Show();
                break;
            case "add":
                Add(commandRest[1]);
                break;
            case "discontinueBook":
                DiscontinueBook(commandRest[1]);
                break;
            case "discontinueAuthor":
                DiscontinueByAuthor(commandRest[1]);
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
            Console.Out.WriteLine("[{0}] {1}: {2} {3}", book.Id, book.Title, book.Author, status);
        }

        Console.Out.WriteLine();
    }

    private void Add(string commandLine)
    {
        var subcommandRest = commandLine.Split(' ');
        AddBook(subcommandRest[0], subcommandRest[1], subcommandRest[2], subcommandRest[3]);
    }

    private void AddBook(string book, string author, string category, string description)
    {
        if (books.Any(b => string.Equals(b.Title, book, StringComparison.OrdinalIgnoreCase)))
        {
            Console.Out.WriteLine("Duplicate book with the name \"{0}\".", book);
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
                Id = NextId(),
                Title = book,
                CategoryId = categoryId,
                Description = description,
                Author = author,
                IsDiscontinued = false,
            }
        );

        Console.Out.WriteLine("Book added: \"{0}\" by {1}", book, author);
    }

    private void DiscontinueBook(string idString)
    {
        var id = long.Parse(idString);
        var book = books.FirstOrDefault(b => b.Id == id);

        book!.IsDiscontinued = true;
        Console.Out.WriteLine("Discontinued book: \"{0}\" by {1} (ID: {2})", book.Title, book.Author, book.Id);
    }

    public void DiscontinueByAuthor(string authorName)
    {
        var authorId = long.Parse(authorName);
        _ = authorId;
    }

    private void Help()
    {
        Console.Out.WriteLine("=== Bookstore Commands ===");
        Console.Out.WriteLine();
        Console.Out.WriteLine("Commands:");
        Console.Out.WriteLine("  show");
        Console.Out.WriteLine("  add <title> <author> <category> <description>");
        Console.Out.WriteLine("  discontinueBook <book-id>");
        Console.Out.WriteLine("  discontinueAuthor <author-name>");
        Console.Out.WriteLine("  help");
        Console.Out.WriteLine("  quit");
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

    private long NextId()
    {
        return ++lastId;
    }
}
