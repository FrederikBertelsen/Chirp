using Chirp.CLI;
using DocoptNet;

class Program
{
    private static readonly string PathToCsvFile = "data/chirp_cli_db.csv";

    //chirp --version in usage useless? still works when removed
    const string usage = @"Chirp.

Usage:
    chirp read
    chirp cheep <message>
    chirp (-h | --help)
    chirp --version

Options:
    -h --help   Show this screen.
    --version   Show version.
";

    public static void Main(string[] args)
    {
        var arguments = new Docopt().Apply(usage, args, version: "Chirp 1.0", exit: true)!;
        if (arguments["read"].IsTrue)
        {
            ReadCheeps();
        }
        else if (arguments["cheep"].IsTrue)
        {
            WriteCheep(arguments["<message>"].ToString());
        }
    }

    /// <summary>
    /// Writes all cheep messages in the csv file to the console
    /// </summary>
    static void ReadCheeps()
    {
        var cheeps = ChirpDataBase.Read(PathToCsvFile);
        UserInterface.PrintCheeps(cheeps);
    }

    /// <summary>
    /// Takes a string message, constructs a Cheep object, and writes it to the csv file
    /// </summary>
    /// <param name="message">The message sent as a cheep, to be written to the csv file</param>
    static void WriteCheep(string message)
    {
        string author = Environment.UserName;
        double timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        Cheep cheep = new Cheep(timestamp, author, message);
        
        ChirpDataBase.Write(PathToCsvFile, cheep);
    }
}