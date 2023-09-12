// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Chirp.CLI;
using DocoptNet;
// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DocoptNet;

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

var arguments = new Docopt().Apply(usage, args, version: "Chirp 1.0", exit: true)!;
if (arguments["read"].IsTrue){
    ReadCheeps();
}
if (arguments["cheep"].IsTrue){
    WriteCheep(arguments["<message>"].ToString());
}
/// <summary>
    /// Writes all cheep messages in the csv file to the console
    /// </summary>
static void ReadCheeps(){
    var cheeps = ChirpDataBase.Read(PathToCsvFile);
    UserInterface.WriteChirp(cheeps);
}
/// <summary>
    /// Takes a string message, constructs a Cheep object, and writes it to the csv file
    /// </summary>
    /// <param name="argText">The message sent as a cheep, to be written to the csv file</param>
static void WriteCheep(string argText){
    string userName = Environment.UserName;
    string text = argText;
    DateTimeOffset timestamp = DateTime.Now;
    Cheep cheep = new Cheep(timestamp.ToUnixTimeSeconds(), userName, text);
    ChirpDataBase.Write(PathToCsvFile, cheep);
}