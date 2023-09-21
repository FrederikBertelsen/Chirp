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
    public void SetFilePath(string filePath);
}

public sealed class CSVDatabase<T> : IDatabase<T>
{
    private static readonly CSVDatabase<T> instance = new CSVDatabase<T>();
    public static CSVDatabase<T> Instance => instance;

    protected string Path;

    /// <summary>
    /// Sets the path of the database.
    /// If the path doesn't lead to a already existing file, it will create a new file at that path
    /// </summary>
    /// <param name="filePath">The file-path that the database will use to store data</param>
    public void SetFilePath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"No file found. Creating new file at '{filePath}'.");

            // create the folders that the file will be in, if the folders doesn't exist
            string dirPath = Regex.Replace(filePath,@"[\w_\-.]+$", "");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // create the csv file at filePath, with the csv Cheep headers
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("Author,Message,Timestamp\n");
            }
        }

        Path = filePath;
    }

    /// <summary>
    /// Constructor for the CSVDatabase
    /// </summary>
    /// <param name="path">path to .csv file</param>
    private CSVDatabase()
    {
        Path = "";
    }

    /// <summary>
    /// returns a list of records with all records of the file or 
    /// the limit
    /// </summary>
    /// <param name="limit">the maximum number of records method will return</param>
    /// <returns>list of records in specified format</returns>
    public IEnumerable<T> Read(int? limit = null)
    {
        List<T> result = new List<T>();
        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (StreamReader reader = new StreamReader(Path))
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
        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            //ensures that we can have variablenames in "Cheep.cs" in lowercase
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            //prevents extra headers
            HasHeaderRecord = false
        };
        //"true" in constructor makes writter append instead of overwrite
        using (StreamWriter writer = new StreamWriter(Path, true))
        using (CsvWriter csv = new CsvWriter(writer, config))
        {
            //creates a list containing only the record to use "WriteRecords", since "WriteRecord" does not create a newline
            csv.WriteRecords(new List<T> { record });
        }
    }
}