using System.Text.RegularExpressions;
using Chirp.CLI;

public class Cheep
{
    public string Author;
    public string Message;
    public double Timestamp;


    /// <summary>
    /// Creates a Cheep object from the data
    /// </summary>
    /// <param name="timestamp">UnixTimeStamp of the Cheep being made</param>
    /// <param name="author">User making the Cheep</param>
    /// <param name="message">Text of the Cheep</param>
    public Cheep(double timestamp, string author, string message)
    {
        Timestamp = timestamp;
        Author = author;
        Message = message;
    }
    /// <summary>
    /// creates an empty Cheep object
    /// </summary>
    public Cheep()
    {
        Author = "";
        Message = "";
        Timestamp = 0;
    }

    /// <summary>
    /// Creates a Cheep object from a csv formatted string
    /// </summary>
    /// <param name="dbOutput">The csv formatted string. Fomatted as username,text,timestamp</param>
    public Cheep(string dbOutput, string author, string message)
    {
        var data = Regex.Split(dbOutput, @",""|"",");
        Author = data[0];
        Message = data[1];
        Timestamp = Double.Parse(data[2]);
    }

    /// <summary>
    /// Returns a formatted string for output
    /// </summary>
    /// <returns>String formatted for output</returns>
    public override string ToString()
    {
        return $"{Author} @ {Utility.UnixTimeStampToDateTime(Timestamp)}: {Message}";
    }
}