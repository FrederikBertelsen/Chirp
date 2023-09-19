using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Chirp.CLI;
using CsvHelper;
using CsvHelper.Configuration;

public class ChirpDataBase
{
    /// <summary>
    /// Writes a Cheep object to .csv file
    /// </summary>
    /// <param name="path">path to .csv file</param>
    /// <param name="cheep">Cheep to be appended to .csv file at path</param>
    public static void Write(string path, Cheep cheep)
    {
        // read current cheeps in .csv file
        List<Cheep> cheepsList = Read(path);
        cheepsList.Add(cheep);

        using StreamWriter writer = new StreamWriter(path);
        using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        // map Cheep class to .csv file format
        csv.Context.RegisterClassMap<CheepMap>();
        
        // write all cheeps to file
        csv.WriteRecords(cheepsList);

        // could not get append to work...
        // using var stream = File.Open(path, FileMode.Append);
        // using var writer = new StreamWriter(stream);
        // var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        // {
        //     // Don't write the header again.
        //     HasHeaderRecord = false,
        // };
        // var csv = new CsvWriter(writer, config);
        // // map Cheep class to .csv file format
        // csv.Context.RegisterClassMap<CheepMap>();
        //
        // // append cheep to .csv file
        // csv.WriteRecord(cheep);
    }

    /// <summary>
    /// returns a list of Cheeps read from a .csv file at path
    /// </summary>
    /// <param name="path">path to .csv file</param>
    /// <returns>list of Cheeps</returns>
    public static List<Cheep> Read(string path)
    {
        using StreamReader sr = new StreamReader(path);
        using CsvReader csv = new CsvReader(sr, CultureInfo.InvariantCulture);
        // map Cheep class to .csv file format
        csv.Context.RegisterClassMap<CheepMap>();
        // read .csv file to Cheeps
        IEnumerable<Cheep>? readCheeps = csv.GetRecords<Cheep>();
        
        return readCheeps.ToList();
    }
}