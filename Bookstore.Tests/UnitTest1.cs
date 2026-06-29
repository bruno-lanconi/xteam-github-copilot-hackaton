using System.IO;
using Tasks;
using AppBookstore = global::Bookstore.Bookstore;

namespace Bookstore.Tests;

public class BookstoreProgramTests
{
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
        var fakeConsole = new FakeConsole(new[] { "add Dune Herbert Invalid desc", "quit" });
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
            "add Dune Herbert Fiction Classic",
            "add Dune SomeoneElse Fiction Another",
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

        Assert.Equal("> > > ", fakeConsole.Writes.ToString());
        Assert.Contains("Duplicate book with the name \"Dune\".", stdOut.ToString());
    }

    [Fact]
    public void Run_DiscontinueBookWithUnknownId_WritesErrorFromExceptionHandler()
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
        Assert.Contains("Error:Object reference not set to an instance of an object.", stdErr.ToString());
    }

    [Fact]
    public void Run_DiscontinueAuthor_WhenNoBooksForAuthor_PrintsNotFoundMessage()
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
        Assert.Contains("Error:The input string 'nobody' was not in a correct format.", stdErr.ToString());
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
