using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DBFacade
{
    private const int CheepsPerPage = 32;
    private const string SqlDbFilePath = "/tmp/chirp.db";
    private SqliteConnection connection;

    public DBFacade()
    {
        connection = Connect();
    }

    private static SqliteConnection Connect()
    {
        SqliteConnection newConnection = new SqliteConnection($"Data Source={SqlDbFilePath}");

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

    public List<CheepViewModel> QueryCheeps(int? page)
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
                "ORDER BY message.pub_date DESC" +
                ") AS cheep" +
                $"WHERE {pageMin} <= ROWID AND ROWID <= {pageMax}";
        }
        else
            sqlQuery =
                "SELECT u.username, m.text, m.pub_date " +
                "FROM message m " +
                "JOIN user u ON u.user_id == m.author_id " +
                "ORDER BY message.pub_date DESC";

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