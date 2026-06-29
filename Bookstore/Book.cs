namespace Bookstore;

public class Book
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long AuthorId { get; set; }
    public int CategoryId { get; set; }
    public string Description { get; set; }
    public bool IsDiscontinued { get; set; }
}