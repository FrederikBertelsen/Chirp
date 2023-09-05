
using System.Text.RegularExpressions;
class Cheep {

    DateTimeOffset timestamp;
    string userName;
    string text;
    /// <summary>
    /// Creates a Cheep object from the data
    /// </summary>
    /// <param name="timestamp">Time of the Cheep being made</param>
    /// <param name="userName">User making the Cheep</param>
    /// <param name="text">Text of the Cheep</param>
    public Cheep(DateTimeOffset timestamp, string userName, string text) {
        this.timestamp = timestamp;
        this.userName = userName;
        this.text = text;
    }

    /// <summary>
    /// Creates a Cheep object from a csv formatted string
    /// </summary>
    /// <param name="dbOutput">The csv formatted string. Fomatted as username,text,timestamp</param>
    public Cheep(string dbOutput) 
    {
        var data = Regex.Split(dbOutput,@",""|"",");
        this.userName = data[0];
        this.text = data[1];
        this.timestamp = Utility.UnixTimeStampToDateTime(Double.Parse(data[2]));
    }

    /// <summary>
    /// Writes this object to the csv file
    /// </summary>
    public void WriteToCSV()
    {
        ChirpDataBase.Write("data/chirp_cli_db.csv", userName + ",\"" + text + "\"," + timestamp.ToUnixTimeSeconds());
    }
    /// <summary>
    /// Returns a formatted string for output
    /// </summary>
    /// <returns>String formatted for output</returns>
    public string ToString() {
        return $"{userName} @ {timestamp}: {text}";
    }
}