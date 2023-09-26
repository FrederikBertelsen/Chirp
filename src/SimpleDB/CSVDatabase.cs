using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SimpleDB;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public interface IDatabase<T>
{
    public IEnumerable<T> Read(int? limit = null);
    public void Store(T record);
}

public sealed class CsvDatabase<T> : IDatabase<T>
{
    public static CsvDatabase<T> Instance { get; } = new CsvDatabase<T>();

    public const string CsvFilePath = "./data/chirp_cli_db.csv";
    
    /// <summary>
    /// Constructor for the CSVDatabase.
    /// This will create a csv file if no file already exists.
    /// </summary>
    private CsvDatabase()
    {
        CreateEmptyCsvFileIfNoneExists();
    }

    private void CreateEmptyCsvFileIfNoneExists()
    {
        // do not create a file if a file already exists
        if (File.Exists(CsvFilePath)) return;
        
        Console.WriteLine($"No csv file found. Creating new csv file at '{CsvFilePath}'.");

        // create the folders that the file will be in, if the folders doesn't exist
        string dirPath = Regex.Replace(CsvFilePath,@"[\w_\-.]+$", "");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // create the csv file at filePath, with the csv Cheep headers
        using (StreamWriter sw = File.CreateText(CsvFilePath))
        {
            sw.WriteLine("Author,Message,Timestamp\n");
        }
    }

    /// <summary>
    /// returns a list of records with all records of the file or 
    /// the limit
    /// </summary>
    /// <param name="limit">the maximum number of records method will return</param>
    /// <returns>list of records in specified format</returns>
    public IEnumerable<T> Read(int? limit = null)
    {
        CreateEmptyCsvFileIfNoneExists();
        
        List<T> result = new List<T>();
        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (StreamReader reader = new StreamReader(CsvFilePath))
        using (CsvReader csv = new CsvReader(reader, config))
        {
            int i = 0;
            foreach (T t in csv.GetRecords<T>())
            {
                if (i >= limit)
                {
                    break;
                }

                result.Add(t);
                i++;
            }
        }

        return result;
    }

    /// <summary>
    /// Writes fields in csv-file from record to file. Not case-sensitive
    /// </summary>
    /// <param name="record">the record that should be appended to the csvfile</param>
    public void Store(T record)
    {
        CreateEmptyCsvFileIfNoneExists();
        
        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            //ensures that we can have variablenames in "Cheep.cs" in lowercase
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            //prevents extra headers
            HasHeaderRecord = false
        };
        //"true" in constructor makes writter append instead of overwrite
        using (StreamWriter writer = new StreamWriter(CsvFilePath, true))
        using (CsvWriter csv = new CsvWriter(writer, config))
        {
            //creates a list containing only the record to use "WriteRecords", since "WriteRecord" does not create a newline
            csv.WriteRecords(new List<T> { record });
        }
    }
}