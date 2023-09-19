
using System.Text.RegularExpressions;
class Cheep {
    public string author{get; set;}
    public string message{get; set;}
    public double timestamp{get; set;}

    /// <summary>
    /// Creates a Cheep object from the data
    /// </summary>
    /// <param name="timestamp">Time of the Cheep being made</param>
    /// <param name="author">User making the Cheep</param>
    /// <param name="message">Text of the Cheep</param>
    public Cheep(double timestamp, string author, string message) {
        this.timestamp = timestamp;
        this.author = author;
        this.message = message;
    }
    
    /// <summary>
    /// Returns a formatted string for output
    /// </summary>
    /// <returns>String formatted for output</returns>
    override public string ToString() {
        return $"{author} @ {Utility.UnixTimeStampToDateTime(timestamp)}: {message}";
    }
}