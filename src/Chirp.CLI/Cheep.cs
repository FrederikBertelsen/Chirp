
using System.Text.RegularExpressions;
public class Cheep {
    public string Author{get; set;}
    public string Message{get; set;}
    public double Timestamp{get; set;}

    /// <summary>
    /// Creates a Cheep object from the data
    /// </summary>
    /// <param name="timestamp">Time of the Cheep being made</param>
    /// <param name="author">User making the Cheep</param>
    /// <param name="message">Text of the Cheep</param>
    public Cheep(double timestamp, string author, string message) {
        this.Timestamp = timestamp;
        this.Author = author;
        this.Message = message;
    }
    
    /// <summary>
    /// Returns a formatted string for output
    /// </summary>
    /// <returns>String formatted for output</returns>
    override public string ToString() {
        return $"{Author} @ {Utility.UnixTimeStampToDateTime(Timestamp)}: {Message}";
    }
}