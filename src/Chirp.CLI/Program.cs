using Chirp.CLI;
using DocoptNet;
using SimpleDB;
class Program
{
    private static IDatabase<Cheep> database = CSVDatabase<Cheep>.Instance;
    private static readonly string PathToCsvFile = "../../data/chirp_cli_db.csv";

    //the amount of cheeps shown when no specific amount is given when reading
    const int standardReadAmount = 10;

    //chirp --version in usage useless? still works when removed
    const string usage = @"Chirp.

Usage:
    chirp read
    chirp read <amount>
    chirp cheep <message>
    chirp (-h | --help)
    chirp --version

Options:
    -h --help   Show this screen.
    --version   Show version.
";

    public static void Main(string[] args)
    {
        database.SetPath(PathToCsvFile);
        var arguments = new Docopt().Apply(usage, args, version: "Chirp 1.0", exit: true)!;
        if (arguments["read"].IsTrue)
        {   
            //Checking for empty string instead of 0 (through AsInt), in the rare case that a user asks for 0 cheeps (ie. "dotnet run read 0")
            //otherwise the standard amount would be shown when asking for zero
            if (arguments["<amount>"].ToString() == ""){
                ReadCheeps(standardReadAmount);
            }
            else {
                ReadCheeps(arguments["<amount>"].AsInt);
            }
        }
        else if (arguments["cheep"].IsTrue)
        {
            WriteCheep(arguments["<message>"].ToString());
        }
        else {
            UserInterface.PrintMessage("Unknown argument. Please use --help or -h for help");
        }
    }

    /// <summary>
    /// Writes all cheep messages in the csv file to the console
    /// </summary>
    static void ReadCheeps(int amountToRead)
    {
        UserInterface.PrintCheeps(database.Read(amountToRead));
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
        
        database.Store(cheep);
    }
}