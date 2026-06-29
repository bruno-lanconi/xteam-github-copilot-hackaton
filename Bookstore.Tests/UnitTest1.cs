using System.IO;
using Tasks;
using AppBookstore = global::Bookstore.Bookstore;

namespace Bookstore.Tests;

public class BookstoreProgramTests
{
    // Command grammar baseline for this feature:
    // create_author <name> <born_date> [awards]
    // add <title> <author_id> <category> <description>

    [Fact]
    public void Run_WithQuit_OnlyWritesPrompt()
    {
        var fakeConsole = new FakeConsole(new[] { "quit" });
        var app = new AppBookstore(fakeConsole);

        app.Run();

        Assert.Equal("> ", fakeConsole.Writes.ToString());
    }

    [Fact]
    public void Run_WithUnknownCommand_WritesExpectedErrorMessage()
    {
        var fakeConsole = new FakeConsole(new[] { "unknown", "quit" });
        var stdOut = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stdOut);

        try
        {
            var app = new AppBookstore(fakeConsole);

            app.Run();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Equal("> > ", fakeConsole.Writes.ToString());
        Assert.Contains("I don't know what the command \"unknown\" is.", stdOut.ToString());
    }

    [Fact]
    public void Run_AddWithInvalidCategory_WritesErrorToStdErr_AndKeepsRunning()
    {
        var fakeConsole = new FakeConsole(new[] { "add Dune 999 Invalid desc", "quit" });
        var stdErr = new StringWriter();
        var originalErr = Console.Error;
        Console.SetError(stdErr);

        try
        {
            var app = new AppBookstore(fakeConsole);

            app.Run();
        }
        finally
        {
            Console.SetError(originalErr);
        }

        Assert.Equal("> > ", fakeConsole.Writes.ToString());
        Assert.Contains("Invalid category", stdErr.ToString());
    }

    [Fact]
    public void Run_AddThenAddDuplicate_PrintsDuplicateWarning()
    {
        var fakeConsole = new FakeConsole(new[]
        {
            "create_author Herbert 1920-10-08 Hugo",
            "add Dune 1 Fiction Classic",
            "add Dune 1 Fiction Another",
            "quit"
        });

        var stdOut = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stdOut);

        try
        {
            var app = new AppBookstore(fakeConsole);

            app.Run();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Equal("> > > > ", fakeConsole.Writes.ToString());
        Assert.Contains("Duplicate book with the name \"Dune\".", stdOut.ToString());
    }

    [Fact]
    public void Run_DiscontinueBookWithUnknownId_WritesNotFoundMessage()
    {
        var fakeConsole = new FakeConsole(new[] { "discontinueBook 999", "quit" });
        var stdErr = new StringWriter();
        var originalErr = Console.Error;
        Console.SetError(stdErr);

        try
        {
            var app = new AppBookstore(fakeConsole);

            app.Run();
        }
        finally
        {
            Console.SetError(originalErr);
        }

        Assert.Equal("> > ", fakeConsole.Writes.ToString());
        Assert.Contains("Book with ID 999 was not found.", stdErr.ToString());
    }

    [Fact]
    public void Run_DiscontinueAuthor_WithInvalidId_PrintsValidationMessage()
    {
        var fakeConsole = new FakeConsole(new[] { "discontinueAuthor nobody", "quit" });
        var stdErr = new StringWriter();
        var originalErr = Console.Error;
        Console.SetError(stdErr);

        try
        {
            var app = new AppBookstore(fakeConsole);

            app.Run();
        }
        finally
        {
            Console.SetError(originalErr);
        }

        Assert.Equal("> > ", fakeConsole.Writes.ToString());
        Assert.Contains("Invalid author id \"nobody\".", stdErr.ToString());
    }

    [Fact]
    public void Run_CreateAuthor_WithValidDate_CreatesAuthor()
    {
        var fakeConsole = new FakeConsole(new[] { "create_author Frank-Herbert 1920-10-08 Hugo;Nebula", "show_authors", "quit" });
        var stdOut = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stdOut);

        try
        {
            var app = new AppBookstore(fakeConsole);
            app.Run();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = stdOut.ToString();
        Assert.Contains("Author created: [1] Frank-Herbert", output);
        Assert.Contains("[1] Frank-Herbert | born: 1920-10-08 | awards: Hugo, Nebula", output);
    }

    [Fact]
    public void Run_CreateAuthor_WithInvalidDate_RejectsInput()
    {
        var fakeConsole = new FakeConsole(new[] { "create_author BadDate 1920/10/08 None", "quit" });
        var stdErr = new StringWriter();
        var originalErr = Console.Error;
        Console.SetError(stdErr);

        try
        {
            var app = new AppBookstore(fakeConsole);
            app.Run();
        }
        finally
        {
            Console.SetError(originalErr);
        }

        Assert.Contains("Invalid born_date \"1920/10/08\". Use YYYY-MM-DD.", stdErr.ToString());
    }

    [Fact]
    public void Run_ShowAuthors_AlwaysIncludesFallbackAuthor()
    {
        var fakeConsole = new FakeConsole(new[] { "show_authors", "quit" });
        var stdOut = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stdOut);

        try
        {
            var app = new AppBookstore(fakeConsole);
            app.Run();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Contains("[0] Unknown Author | born: 1900-01-01 | awards: (none)", stdOut.ToString());
    }

    [Fact]
    public void Run_AddWithValidAuthorId_ShowsResolvedAuthorName()
    {
        var fakeConsole = new FakeConsole(new[]
        {
            "create_author Herbert 1920-10-08 Hugo",
            "add Dune 1 Fiction Classic",
            "show",
            "quit"
        });

        var stdOut = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stdOut);

        try
        {
            var app = new AppBookstore(fakeConsole);
            app.Run();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = stdOut.ToString();
        Assert.Contains("Book added: \"Dune\" by Herbert", output);
        Assert.Contains("[1] Dune: Herbert", output);
    }

    [Fact]
    public void Run_AddWithInvalidAuthorId_UsesFallbackUnknownAuthor()
    {
        var fakeConsole = new FakeConsole(new[]
        {
            "add LostBook 999 Fiction Mystery",
            "show",
            "quit"
        });

        var stdOut = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stdOut);

        try
        {
            var app = new AppBookstore(fakeConsole);
            app.Run();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = stdOut.ToString();
        Assert.Contains("Book added: \"LostBook\" by Unknown Author", output);
        Assert.Contains("[1] LostBook: Unknown Author", output);
    }

    private sealed class FakeConsole : IConsole
    {
        private readonly Queue<string> _inputs;

        public FakeConsole(IEnumerable<string> inputs)
        {
            _inputs = new Queue<string>(inputs);
        }

        public StringWriter Writes { get; } = new();

        public string ReadLine()
        {
            return _inputs.Dequeue();
        }

        public void Write(string format, params object[] args)
        {
            Writes.Write(string.Format(format, args));
        }

        public void WriteLine(string format, params object[] args)
        {
            Writes.WriteLine(string.Format(format, args));
        }

        public void WriteLine()
        {
            Writes.WriteLine();
        }
    }
}
