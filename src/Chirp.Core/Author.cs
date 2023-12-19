namespace Chirp.Core;

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();
}