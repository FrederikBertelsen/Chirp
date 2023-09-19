
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
namespace SimpleDB;
public interface IDatabase<T>
{
    public IEnumerable<T> Read(int? limit = null);
    public void Store(T record);
}
public sealed class CSVDatabase<T> : IDatabase<T>
{
    private string path;
    
    public CSVDatabase (string path)
    {
        this.path = path;
    }
    
    /// <summary>
    /// returns
    /// </summary>
    /// <param name="path">path to .csv file</param>
    /// <returns>list of all but first line of the .csv file</returns>
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
    /// Writes toString to file
    /// </summary>
    /// <param name="record">string to be appended to file at path</param>
    public void Store(T record)
    {
        CsvConfiguration config = new (CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (StreamWriter writer = new (path))
        using (CsvWriter csv = new(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecord(record);
        }
    }
}