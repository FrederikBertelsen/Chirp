
using System.Text.RegularExpressions;
class Cheep {

    public double timestamp{get; set;}
    public string author{get; set;}
    public string message{get; set;}
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
    /// Creates a Cheep object from a csv formatted string
    /// </summary>
    /// <param name="dbOutput">The csv formatted string. Fomatted as author,message,timestamp</param>
    public Cheep(string dbOutput) 
    {
        var data = Regex.Split(dbOutput,@",""|"",");
        this.author = data[0];
        this.message = data[1];
        this.timestamp = Double.Parse(data[2]);
    }

    /// <summary>
    /// Writes this object to the csv file
    /// </summary>
    public void WriteToCSV()
    {
        ChirpDataBase.Write("data/chirp_cli_db.csv", author + ",\"" + message + "\"," + timestamp);
    }
    /// <summary>
    /// Returns a formatted string for output
    /// </summary>
    /// <returns>String formatted for output</returns>
    override public string ToString() {
        return $"{author} @ {Utility.UnixTimeStampToDateTime(timestamp)}: {message}";
    }
}