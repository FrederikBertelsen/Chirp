using Chirp.Razor;
using Chirp.Razor.Pages;
using Microsoft.EntityFrameworkCore;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string authorName, int page);
}

public class CheepService : ICheepService
{
    /// <summary>
    /// Returns a list of all Cheep Records in the DB
    /// </summary>
    /// <returns>A list of Cheep Records</returns>
    public List<CheepViewModel> GetCheeps(int page)
    {
        using ChirpDBContext db = new ChirpDBContext();
        CheepRepository cheepRepository = new CheepRepository(db);


        List<Cheep> dbCheepList = (List<Cheep>)cheepRepository.GetPageOfCheeps(page);
        return Utility.DbCheepsToRecordCheeps(dbCheepList);
    }

    /// <summary>
    /// Returns a list of Cheep Records written by an author
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>A list of Cheep Records written by the author</returns>
    public List<CheepViewModel> GetCheepsFromAuthor(string authorName, int page)
    {
        using ChirpDBContext db = new ChirpDBContext();
        CheepRepository cheepRepository = new CheepRepository(db);

        List<Cheep> dbCheepList = (List<Cheep>)cheepRepository.GetPageOfCheepsByAuthor(authorName, page);

        return Utility.DbCheepsToRecordCheeps(dbCheepList);
    }
}