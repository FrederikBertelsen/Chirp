// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Chirp.CLI;

if(args.Length == 0)
    return;
if(args[0].ToLower() == "read")
{
    var sr = new StreamReader("data/chirp_cli_db.csv");
    
    sr.ReadLine();
    List<Cheep> cheeps = new List<Cheep>();
    while(sr.Peek() >= 0)
    {
        Cheep chirp = new Cheep(sr.ReadLine());
        cheeps.Add(chirp);
    }
    UserInterface.Writechirp(cheeps);
}


if (args[0].ToLower() == "cheep") 
{
    if (args[1] == null)
    {
        Console.WriteLine("Text cannot be emtpy!");
        return;
    }
    string userName = Environment.UserName;
    string text = args[1];
    DateTimeOffset timestamp = DateTime.Now;
    Cheep chirp = new Cheep(timestamp, userName, text);
    chirp.WriteToCSV();
}