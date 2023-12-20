namespace Chirp.Core;

public class Author
{
    /// <summary>
    /// the unique id of the author
    /// </summary>
    public int AuthorId { get; set; }
    
    /// <summary>
    /// the name of the author
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// the list of all cheeps written by the author
    /// </summary>
    public List<Cheep> Cheeps { get; set; } = new();
}