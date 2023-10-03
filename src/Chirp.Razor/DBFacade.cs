using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;

public class DBFacade
{
    private const int CheepsPerPage = 32;
    
    private SqliteConnection connection;



    public DBFacade()
    {   
        string? dbFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        //if the environment variable is not set properly/doesn't exist, the temporary directory is used instead
        if (dbFilePath == null){
            dbFilePath = Path.GetTempPath() + "chirp.db";
        }
        connection = Connect(dbFilePath);
        if (!isDBFilled()){
            fillDB();
            Console.WriteLine("dwadwa 9999999999999");
        }
        else {
                        Console.WriteLine("dwadwa 123123123123123");

        }
        }

    private static SqliteConnection Connect(string filePath)
    {
        Console.WriteLine(filePath);
        SqliteConnection newConnection = new SqliteConnection($"Data Source={filePath}");

        try
        {   
            newConnection.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }



        return newConnection;
    }

    public bool isDBFilled(){
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND (name='message' OR name='user')";
        using SqliteDataReader sqlReader = command.ExecuteReader();
        int i = 0;
        while(sqlReader.Read()){
            i++;
        }
        return i==2;
    }

    public void fillDB(){
        Console.WriteLine("filling DB!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        EmbeddedFileProvider schemaEmbeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var reader = schemaEmbeddedProvider.GetFileInfo("./data/schema.sql").CreateReadStream();
        using var schemaSr = new StreamReader(reader);
        string schemaQuery = schemaSr.ReadToEnd();
        SqliteCommand schemaCmd = connection.CreateCommand();
        schemaCmd.CommandText = schemaQuery;
        schemaCmd.ExecuteNonQuery();

        EmbeddedFileProvider dumpEmbeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var dumpReader = dumpEmbeddedProvider.GetFileInfo("./data/dump.sql").CreateReadStream();
        using var dumpSr = new StreamReader(reader);
        string dumpQuery = dumpSr.ReadToEnd();
        SqliteCommand dumpCmd = connection.CreateCommand();
        dumpCmd.CommandText = dumpQuery;
        dumpCmd.ExecuteNonQuery();
    }

    public List<CheepViewModel> QueryCheeps(int? page = null)
    {
        string sqlQuery;
        if (page != null && page > 0)
        {
            int? pageMin = 32*(page-1);
            int? pageMax = 32*page-1;
            sqlQuery =
                "SELECT cheep.username, cheep.text, cheep.pub_date" +
                "FROM (" +
                "SELECT u.username, m.text, m.pub_date" +
                "FROM message m" +
                "JOIN user u ON u.user_id == m.author_id" +
                "ORDER BY m.pub_date DESC" +
                ") AS cheep" +
                $"WHERE {pageMin} <= ROWID AND ROWID <= {pageMax}";
        }
        else
            sqlQuery =
                "SELECT u.username, m.text, m.pub_date " +
                "FROM message m " +
                "JOIN user u ON u.user_id == m.author_id " +
                "ORDER BY m.pub_date DESC";

        List<CheepViewModel> outputCheepViewModels = new List<CheepViewModel>();

        var command = connection.CreateCommand();
        command.CommandText = sqlQuery;

        using SqliteDataReader sqlReader = command.ExecuteReader();
        while (sqlReader.Read())
        {
            string author = sqlReader.GetString(0);
            string message = sqlReader.GetString(1);

            int timeStamp = sqlReader.GetInt32(2);
            string timestamp = Utility.UnixTimeStampToFormatString(timeStamp);

            CheepViewModel newCheep = new CheepViewModel(author, message, timestamp);
            outputCheepViewModels.Add(newCheep);
        }

        return outputCheepViewModels;
    }
}