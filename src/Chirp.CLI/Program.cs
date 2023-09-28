using System.Net.Http.Headers;
using System.Net.Http.Json;
using Chirp.CLI;
using DocoptNet;
using SimpleDB;
class Program
{
    //the amount of cheeps shown when no specific amount is given when reading
    const int standardReadAmount = 10;

    //client has to be static, as it is used in the Main method, which is static
    static HttpClient client = new();

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
        //setup for server
        client = new HttpClient();
        string baseURL = "http://localhost:5277";
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        IDictionary<string, ValueObject>? arguments = new Docopt().Apply(usage, args, version: "Chirp 1.0", exit: true)!;
        if (arguments["read"].IsTrue && (arguments["<amount>"].IsInt || arguments["<amount>"].IsNullOrEmpty))
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
        //Ideally we would await, but for now we just sleep, so that read/write methods (which are async) have time to complete
        Thread.Sleep(1000);
    }

    /// <summary>
    /// Gets a specified number of cheeps from the server, and writes them to the console. If no amount is specified,
    /// all cheeps are returned.
    /// </summary>
    /// <param name="amountToRead">The amount of cheeps requested from the server. If there are less than this number in
    /// the server's csv file, the max amount is returned</param>

     static async void ReadCheeps(int amountToRead)
    {   
        try {
        HttpResponseMessage response = await client.GetAsync("cheeps/"+amountToRead);
        response.EnsureSuccessStatusCode();
        List<Cheep> cheepList = await response.Content.ReadFromJsonAsync<List<Cheep>>();
        UserInterface.PrintCheeps(cheepList);
        }
        catch (HttpRequestException e){
        UserInterface.PrintError("Failed to read cheeps, with error message: " + e.ToString());
        }
    }

    /// <summary>
    /// Takes a string message, constructs a Cheep object, and posts it to the server, where it is stored in the csv file
    /// </summary>
    /// <param name="message">The message sent as a cheep, to be written to the csv file on the server</param>
    static async void WriteCheep(string message)
    {
        string author = Environment.UserName;
        double timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        Cheep cheep = new Cheep(timestamp, author, message);
        try {
            HttpResponseMessage response = await client.PostAsJsonAsync("cheep", cheep);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e){
            UserInterface.PrintError("Failed to cheep your cheep, with error message: " + e.ToString());
        }

    }
}
