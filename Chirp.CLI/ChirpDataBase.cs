using System.Text;
using System.Text.RegularExpressions;

public class ChirpDataBase{
    public void Write(string path, string cheep){
        File.AppendAllText(path, $"\n{cheep}");
    }

    public List<string> Read(string path){
        var sr = new StreamReader(path);
        var result = new List<string>();
        //removing first unneeded line
        sr.ReadLine();
        while(sr.Peek() >= 0)
        {
            result.Add(sr.ReadLine());
        }
        return result;
    }
}