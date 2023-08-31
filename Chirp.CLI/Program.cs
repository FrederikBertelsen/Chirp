// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

if(args.Length == 0)
    return;
if(args[0] == "read")
{
    var sr = new StreamReader("data/chirp_cli_db.csv");
    
    sr.ReadLine();
    while(sr.Peek() >= 0)
    {
        var data = Regex.Split(sr.ReadLine(),@",""|"",");
        Console.WriteLine($"{data[0]} @ {data[2]}: {data[1]}");
    }
}