using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    // IEnumerable<Cheep> GetCheepsByAuthor(string authorName);
    IEnumerable<Cheep> GetPageOfCheepsByAuthor(string authorName, int pageNumber);

    // IEnumerable<Cheep> GetAllCheeps();
    IEnumerable<Cheep> GetPageOfCheeps(int pageNumber);
    void AddCheep(string authorName, string text, DateTime timestamp);
    void RemoveCheep(int cheepId);
    void RemoveAuthor(string authorName);
}

public class CheepRepository : ICheepRepository
{
    public const int PageSize = 32;

    private readonly ChirpDBContext dbContext;

    /// <summary>
    /// This Repository contains all direct communication with the database.
    /// We use this Repository to abstract hte data access layer from the rest of the application.
    /// </summary>
    /// <param name="dbContext">The ChirpDBContext that will be injected into this repo</param>
    public CheepRepository(ChirpDBContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // public IEnumerable<Cheep> GetCheepsByAuthor(string authorName)
    // {
    //     Author author = dbContext.Authors
    //         .Where(a => a.Name == authorName)
    //         .Include(a => a.Cheeps)
    //         .Single();
    //
    //     List<Cheep> orderedCheeps = author.Cheeps.OrderBy(c => c.TimeStamp).ToList();
    //     return orderedCheeps;
    // }

    /// <summary>
    /// Returns a list of Cheeps (size: PageSize) written by the specified Author sorted by time posted.
    /// </summary>
    /// <param name="authorName">The name of the Author</param>
    /// <param name="pageNumber">The page number (starts at 1)</param>
    /// <returns>A IEnumerable of Cheeps</returns>
    public IEnumerable<Cheep> GetPageOfCheepsByAuthor(string authorName, int pageNumber)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");

        Author? author = dbContext.Authors
            .Where(a => a.Name == authorName)
            .Include(a => a.Cheeps)
            .SingleOrDefault();
        
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");
        
        int skipCount = (pageNumber - 1) * PageSize;

        List<Cheep> orderedCheeps = author.Cheeps
            .OrderBy(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .ToList();

        return orderedCheeps;
    }

    // public IEnumerable<Cheep> GetAllCheeps()
    // {
    //     List<Cheep> dbCheepList = dbContext.Cheeps
    //         .Include(c => c.Author)
    //         .OrderBy(c => c.TimeStamp)
    //         .ToList();
    //
    //     return dbCheepList;
    // }

    /// <summary>
    /// Returns a list of Cheeps (size: PageSize) sorted by time posted.
    /// </summary>
    /// <param name="pageNumber">The page number (starts at 1)</param>
    /// <returns></returns>
    public IEnumerable<Cheep> GetPageOfCheeps(int pageNumber)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");
        
        int skipCount = (pageNumber - 1) * PageSize;

        List<Cheep> dbCheepList = dbContext.Cheeps
            .Skip(skipCount)
            .Take(PageSize)
            .ToList();
        
        return dbCheepList;
    }

    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="authorName">The name of the Author of the Cheep</param>
    /// <param name="text">The text in the Cheep</param>
    /// <param name="timestamp">The time the Cheep was posted</param>
    public void AddCheep(string authorName, string text, DateTime timestamp)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (text is null)
            throw new ArgumentNullException(nameof(text));
        
        Author? author = dbContext.Authors.SingleOrDefault(a => a.Name == authorName);
        
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");
        
        Cheep newCheep = new Cheep() { Author = author, Text = text, TimeStamp = timestamp };
        
        dbContext.Cheeps.Add(newCheep);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Removes a Cheep from the database.
    /// </summary>
    /// <param name="cheepId">The ID of the Cheep</param>
    public void RemoveCheep(int cheepId)
    {
        Cheep? cheep = dbContext.Cheeps.SingleOrDefault(c => c.CheepId == cheepId);
        
        if (cheep is null)
            throw new ArgumentException($"Cheep with ID '{cheepId}' not found.");
        
        dbContext.Cheeps.Remove(cheep);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Removes an Author and all of their Cheeps.
    /// </summary>
    /// <param name="authorName">The name of the Author</param>
    public void RemoveAuthor(string authorName)
    {
        Author? author = dbContext.Authors
            .Where(a => a.Name == authorName)
            .Include(a => a.Cheeps)
            .SingleOrDefault();

        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");
        
        dbContext.Cheeps.RemoveRange(author.Cheeps);
        dbContext.Authors.Remove(author);
        dbContext.SaveChanges();
    }
}