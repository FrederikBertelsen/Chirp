namespace Chirp.CLI;

public class UserInterface
{
    /// <summary>
    /// Takes a cheep and writes its to string in the console 
    /// </summary>
    /// <param name="ch">The given cheep that will be written in the console</param>
    public static void WriteChirp(IEnumerable<Cheep> ch)
    {
        foreach (Cheep value in ch){
           Console.WriteLine(value.ToString());
        }
    }
    
}