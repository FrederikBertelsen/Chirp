namespace Chirp.CLI;

public class UserInterface
{
    /// <summary>
    /// Takes a list of cheeps and writes them as strings in the console 
    /// </summary>
    /// <param name="cheeps">The given cheep that will be written in the console</param>
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps){
           Console.WriteLine(cheep.ToString());
        }
    }
    
}