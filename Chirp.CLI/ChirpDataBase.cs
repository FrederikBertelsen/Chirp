using System.Text;
using System.Text.RegularExpressions;

public class ChirpDataBase{
    /// <summary>
    /// Writes datastring to file
    /// </summary>
    /// <param name="path">path to .csv file</param>
    /// <param name="data">string to be appended to file at path</param>
    public static void Write(string path, string data){
        File.AppendAllText(path, $"\n{data}");
    }
    /// <summary>
    /// returns all but first line from given path
    /// </summary>
    /// <param name="path">path to .csv file</param>
    /// <returns>list of all but first line of the .csv file</returns>
    public static List<string> Read(string path){
        var sr = new StreamReader(path);
        var result = new List<string>();
        //removing first unneeded line
        sr.ReadLine();
        while(sr.Peek() >= 0)
        {
            //result.Add(sr.ReadLine());
        }
        return result;
    }
}