// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SimpleDB;

if(args.Length == 0)
    return;
if(args[0].ToLower() == "read")
{
    int limit;
    if (args.Length < 2)
    {
        limit = 10;
    }else 
    {
        limit = int.Parse(args[1]);
    }
    IDatabase<Cheep> reader = new CSVDatabase<Cheep>("data/chirp_cli_db.csv");
    foreach(Cheep cheep in reader.Read(limit))
    {
        Console.WriteLine(cheep);
    }
}


if (args[0].ToLower() == "cheep") 
{
    IDatabase<Cheep> reader = new CSVDatabase<Cheep>("data/chirp_cli_db.csv");
    if (args[1] == null)
    {
        Console.WriteLine("Text cannot be emtpy!");
        return;
    }
    string userName = Environment.UserName;
    string text = args[1];
    DateTimeOffset timestamp = DateTime.Now;
    Cheep cheep = new Cheep(timestamp.ToUnixTimeSeconds(), userName, text);
    reader.Store(cheep);
}