using System.Collections.Generic;
using System.Threading.Tasks;
using Chirp.Razor.Pages;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    Task<IEnumerable<CheepViewModel>> GetPageOfCheepsByAuthor(string authorName, int pageNumber);

    Task<IEnumerable<CheepViewModel>> GetPageOfCheeps(int pageNumber);
    Task CreateCheep(string authorName, string text, DateTime timestamp);
    Task RemoveCheep(int cheepId);
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

    /// <summary>
    /// Returns a list of Cheeps (size: PageSize) written by the specified Author sorted by time posted.
    /// </summary>
    /// <param name="authorName">The name of the Author</param>
    /// <param name="pageNumber">The page number (starts at 1)</param>
    /// <returns>A IEnumerable of Cheeps</returns>
    public async Task<IEnumerable<CheepViewModel>> GetPageOfCheepsByAuthor(string authorName, int pageNumber)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");

        int skipCount = (pageNumber - 1) * PageSize;

        List<CheepViewModel> cheepList = await dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == authorName)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, c.TimeStamp))
            .ToListAsync();

        return cheepList;
    }

    /// <summary>
    /// Returns a list of Cheeps (size: PageSize) sorted by time posted.
    /// </summary>
    /// <param name="pageNumber">The page number (starts at 1)</param>
    /// <returns></returns>
    public async Task<IEnumerable<CheepViewModel>> GetPageOfCheeps(int pageNumber)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");

        int skipCount = (pageNumber - 1) * PageSize;

        List<CheepViewModel> cheepList = await dbContext.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, c.TimeStamp))
            .ToListAsync();

        return cheepList;
    }

    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="authorName">The name of the Author of the Cheep</param>
    /// <param name="text">The text in the Cheep</param>
    /// <param name="timestamp">The time the Cheep was posted</param>
    public async Task CreateCheep(string authorName, string text, DateTime timestamp)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);

        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        Cheep newCheep = new Cheep() { Author = author, Text = text, TimeStamp = timestamp };

        dbContext.Cheeps.Add(newCheep);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a Cheep from the database.
    /// </summary>
    /// <param name="cheepId">The ID of the Cheep</param>
    public async Task RemoveCheep(int cheepId)
    {
        Cheep? cheep = await dbContext.Cheeps.SingleOrDefaultAsync(c => c.CheepId == cheepId);

        if (cheep is null)
            throw new ArgumentException($"Cheep with ID '{cheepId}' not found.");

        dbContext.Cheeps.Remove(cheep);
        await dbContext.SaveChangesAsync();
    }
}