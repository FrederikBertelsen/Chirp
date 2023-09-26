
using System.Globalization;
using System.Text.RegularExpressions;
public record class Cheep {
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
        if (author is null)
            throw new ArgumentNullException(nameof(author));
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (author == "")
            throw new ArgumentException("author can not be empty");
        if (message == "")
            throw new ArgumentException("message can not be empty");
        if (timestamp < 0)
            throw new ArgumentException("timestamp can not be negative");
   
        
        Timestamp = timestamp;
        Author = author;
        Message = message;
    }
    
    /// <summary>
    /// Returns a formatted string for output
    /// </summary>
    /// <returns>String formatted for output</returns>
    public override string ToString()
    {
        string output = $"{Author} @ {Utility.UnixTimeStampToDateTime(Timestamp).ToString(CultureInfo.InvariantCulture)}: {Message}";
        Regex.Replace(output, @"\/", @"\-");
        return output;
    }
}