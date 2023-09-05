// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Text.RegularExpressions;

if(args.Length == 0)
    return;
if(args[0].ToLower() == "read")
{
    var sr = new StreamReader("data/chirp_cli_db.csv");
    
    sr.ReadLine();
    while(sr.Peek() >= 0)
    {
        Cheep chirp = new Cheep(sr.ReadLine());
        Console.WriteLine(chirp.ToString());
    }
}


if (args[0].ToLower() == "cheep") 
{
    if (args[1] == null)
    {
        Console.WriteLine("Text cannot be emtpy!");
        return;
    }
    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    string text = args[1];
    DateTimeOffset timestamp = DateTime.Now;
    Cheep chirp = new Cheep(timestamp, userName, text);
    chirp.WriteToCSV();
}