namespace Chirp.Core;

public class Cheep
{
    /// <summary>
    //// the unique id of the author
    /// </summary>
    public int CheepId { get; set; }
    
    /// <summary>
    /// the text content of the cheep
    /// </summary>
    public required string Text { get; set; }
    
    /// <summary>
    /// the timestamp taken when the cheep is posted
    /// </summary>
    public required DateTime TimeStamp { get; set; }
    
    /// <summary>
    /// the id of the author that posted the cheep
    /// </summary>
    public int AuthorId { get; set; }
    
    /// <summary>
    /// f the author that posted the cheep
    /// </summary>
    public required Author Author { get; set; }
}