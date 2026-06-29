namespace Bookstore;

public class Author
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string BornDate { get; set; }
    public List<string> AwardsList { get; set; } = new();
}
