using Chirp.Razor;
using SimpleDB;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private static HttpClient client = new();

    private static DBFacade facade = new();

    public List<CheepViewModel> GetCheeps()
    {
        return facade.QueryCheeps();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return null;
    }
}
