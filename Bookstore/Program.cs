using Tasks;

namespace Bookstore
{
	public sealed class Bookstore
	{
		private const string QUIT = "quit";
		private readonly IConsole console;

		private IList<Book> Books = new List<Book>();
	 		
		// Dicionário de categorias: ID -> Nome
		private IReadOnlyDictionary<int, string> CategoryDictionary = new Dictionary<int, string>
		{
			{ 1, "Fiction" },
			{ 2, "Romance" },
			{ 3, "Science Fiction" },
			{ 4, "Fantasy" },
			{ 5, "Mystery" },
			{ 6, "Biography" }
		};

		// Dicionário reverso para lookup eficiente: Nome -> ID
		private IReadOnlyDictionary<string, int> CategoryLookup;

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new Bookstore(new RealConsole()).Run();
		}
		
		public Bookstore(IConsole console)
		{
			this.console = console;
			InitializeCategoryLookup();
		}

		/// <summary>Inicializa o dicionário reverso de categorias para lookup case-insensitive</summary>
		private void InitializeCategoryLookup()
		{
			var lookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			foreach (var kvp in CategoryDictionary)
			{
				lookup[kvp.Value] = kvp.Key;
			}
			CategoryLookup = lookup;
		}

		public void Run()
		{
			while (true) {
				try
				{
					console.Write("> ");
					var command = console.ReadLine();
					if (command == QUIT) {
						break;
					}
					Execute(command);
				} 
				catch (FormatException)
				{
					console.WriteLine("Error: Invalid command format or ID");
				}
				catch (IndexOutOfRangeException)
				{
					console.WriteLine("Error: Missing command arguments");
				}
				catch (Exception e)
				{
					console.WriteLine("Error: {0}", e.Message);
				}
			}
		}


		private void Execute(string commandLine)
		{
			if (string.IsNullOrWhiteSpace(commandLine))
			{
				return;
			}

			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			
			try
			{
				switch (command)
				{
					case "show":
						Show();
						break;
					case "add":
						if (commandRest.Length < 2)
						{
							console.WriteLine("Error: 'add' command requires arguments");
							break;
						}
						Add(commandRest[1]);
						break;
					case "discontinueBook":
						if (commandRest.Length < 2)
						{
							console.WriteLine("Error: 'discontinueBook' command requires a book ID");
							break;
						}
						DiscontinueBook(commandRest[1]);
						break;
					case "discontinueAuthor":
						if (commandRest.Length < 2)
						{
							console.WriteLine("Error: 'discontinueAuthor' command requires an author name");
							break;
						}
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
			catch (Exception ex)
			{
				console.WriteLine("Error executing command: {0}", ex.Message);
			}
		}


		private void Show()
		{
			if (Books.Count == 0)
			{
				console.WriteLine("No books in the store.");
				return;
			}

			foreach (var book in Books)
			{
				string status = book.IsDiscontinued ? "(discontinued)" : "";
				console.WriteLine("[{0}] {1}: {2} {3}", book.Id, book.Title, book.Author, status);
			}
			console.WriteLine();
		}


		private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray());
			
			if (subcommandRest.Length < 4)
			{
				console.WriteLine("Error: 'add' command requires: title author category description");
				console.WriteLine("Example: add \"Book Title\" \"Author Name\" \"Fiction\" \"Book description\"");
				return;
			}

			AddBook(subcommandRest[0], subcommandRest[1], subcommandRest[2], subcommandRest[3]);
		}


		private void AddBook(string book, string author, string category, string description)
		{
			if (string.IsNullOrWhiteSpace(book) || string.IsNullOrWhiteSpace(author) || 
				string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(description))
			{
				console.WriteLine("Error: All fields (title, author, category, description) are required");
				return;
			}

			if (Books.Any(b => string.Equals(b.Title, book, StringComparison.OrdinalIgnoreCase)))
			{
				console.WriteLine("Error: Duplicate book with the name \"{0}\".", book);
				return;
			}

			if (!CategoryLookup.TryGetValue(category, out int categoryId))
			{
				console.WriteLine("Error: Invalid category \"{0}\". Valid categories are:", category);
				foreach (var cat in CategoryDictionary.Values)
				{
					console.WriteLine("  - {0}", cat);
				}
				return;
			}

			Books.Add(new Book
			{
				Id = NextId(), 
				Title = book, 
				CategoryId = categoryId, 
				Description = description, 
				Author = author,
				IsDiscontinued = false
			});

			console.WriteLine("Book added: \"{0}\" by {1}", book, author);
		}


		private void DiscontinueBook(string idString)
		{
			if (!long.TryParse(idString, out long id))
			{
				console.WriteLine("Error: Invalid book ID. Expected a number.");
				return;
			}

			var book = Books.FirstOrDefault(b => b.Id == id);

			if (book == null)
			{
				console.WriteLine("Error: Book with ID {0} not found.", id);
				return;
			}

			if (book.IsDiscontinued)
			{
				console.WriteLine("Book \"{0}\" is already discontinued.", book.Title);
				return;
			}

			book.IsDiscontinued = true;
			console.WriteLine("Discontinued book: \"{0}\" by {1} (ID: {2})", book.Title, book.Author, book.Id);
		}
		
		public void DiscontinueByAuthor(string authorName)
		{
			if (string.IsNullOrWhiteSpace(authorName))
			{
				console.WriteLine("Error: Author name is required.");
				return;
			}

			var booksToDiscontinue = Books
				.Where(b => string.Equals(b.Author.Trim(), authorName.Trim(), StringComparison.OrdinalIgnoreCase) && !b.IsDiscontinued)
				.ToList();

			if (booksToDiscontinue.Count == 0)
			{
				console.WriteLine("Error: No active books found by author \"{0}\".", authorName);
				return;
			}

			foreach (var book in booksToDiscontinue)
			{
				book.IsDiscontinued = true;
				console.WriteLine("Discontinued: \"{0}\" by {1} (ID: {2})", book.Title, book.Author, book.Id);
			}

			console.WriteLine("Total discontinued: {0} book(s) by {1}", booksToDiscontinue.Count, authorName);
		}


		private void Help()
		{
			console.WriteLine("=== Bookstore Commands ===");
			console.WriteLine();
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("    - Display all books in the store");
			console.WriteLine();
			console.WriteLine("  add <title> <author> <category> <description>");
			console.WriteLine("    - Add a new book to the store");
			console.WriteLine("    - Example: add \"The Hobbit\" \"J.R.R. Tolkien\" \"Fantasy\" \"Adventure novel\"");
			console.WriteLine();
			console.WriteLine("  discontinueBook <book-id>");
			console.WriteLine("    - Discontinue a specific book by its ID");
			console.WriteLine("    - Example: discontinueBook 1");
			console.WriteLine();
			console.WriteLine("  discontinueAuthor <author-name>");
			console.WriteLine("    - Discontinue all books by a specific author");
			console.WriteLine("    - Example: discontinueAuthor \"J.R.R. Tolkien\"");
			console.WriteLine();
			console.WriteLine("  help");
			console.WriteLine("    - Show this help message");
			console.WriteLine();
			console.WriteLine("  quit");
			console.WriteLine("    - Exit the program");
			console.WriteLine();
			console.WriteLine("Available categories:");
			foreach (var category in CategoryDictionary.Values)
			{
				console.WriteLine("  - {0}", category);
			}
			console.WriteLine();
		}

		private void Error(string command)
		{
			console.WriteLine("Error: Unknown command \"{0}\". Type 'help' for available commands.", command);
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}
