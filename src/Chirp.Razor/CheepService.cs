using Chirp.Razor;
using Chirp.Razor.Pages;
using Microsoft.EntityFrameworkCore;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string authorName);
}

public class CheepService : ICheepService
{
    /// <summary>
    ///     Returns a list of all Cheep Records in the DB
    /// </summary>
    /// <returns>A list of Cheep Records</returns>
    public List<CheepViewModel> GetCheeps()
    {
        using var db = new ChirpDBContext();

        var dbCheepList = db.Cheeps
            .Include(c => c.Author)
            .OrderBy(c => c.TimeStamp)
            .ToList();

        return Utility.DbCheepsToRecordCheeps(dbCheepList);
    }

    /// <summary>
    ///     Returns a list of Cheep Records written by an author
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>A list of Cheep Records written by the author</returns>
    public List<CheepViewModel> GetCheepsFromAuthor(string authorName)
    {
        using var db = new ChirpDBContext();

        var author = db.Authors
            .Where(a => a.Name == authorName)
            .Include(a => a.Cheeps)
            .Single();

        var orderedCheeps = author.Cheeps.OrderBy(c => c.TimeStamp).ToList();

        return Utility.DbCheepsToRecordCheeps(orderedCheeps);
    }
}