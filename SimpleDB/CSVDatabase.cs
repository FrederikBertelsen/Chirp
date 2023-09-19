namespace SimpleDB;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public interface IDatabase<T>
{
    public IEnumerable<T> Read(int? limit = null);
    public void Store(T record);
}
public sealed class CSVDatabase<T> : IDatabase<T>
{
    private string path;
    /// <summary>
    /// Constructor for the CSVDatabase
    /// </summary>
    /// <param name="path">path to .csv file</param>
    public CSVDatabase (string path)
    {
        this.path = path;
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
        using (StreamReader reader = new StreamReader(path))
        using (CsvReader csv = new CsvReader(reader, config))
        {
            int i=0;
            foreach(T t in csv.GetRecords<T>())
            {
                result.Add(t);
                if(i>limit)
                {
                    break;
                }
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
        using (StreamWriter writer = new StreamWriter(path, true))
        using (CsvWriter csv = new CsvWriter(writer, config))
        {
            //creates a list containing only the record to use "WriteRecords", since "WriteRecord" does not create a newline
            csv.WriteRecords(new List<T> {record});
        }
    }
}