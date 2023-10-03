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

    public List<CheepViewModel> GetCheeps()
    {
        return null;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return null;
    }
}
