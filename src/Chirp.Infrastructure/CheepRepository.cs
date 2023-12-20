using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext dbContext;

    const int PageSize = 32;

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
    public async Task<IEnumerable<CheepDTO>> GetPageOfCheepsByAuthor(string authorName, int pageNumber)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        int skipCount = (pageNumber - 1) * PageSize;

        List<CheepDTO> cheepList = await dbContext.Cheeps
            .Where(c => c.AuthorId == author.AuthorId)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .Select(c => new CheepDTO(author.Name, c.Text, c.TimeStamp, c.CheepId))
            .ToListAsync();
        return cheepList;
    }

    /// <summary>
    /// Get a list of length pageSize containing cheepsDTOs written by the people the author is following.
    /// Where pageNumber determines which page of pageSize is returned.
    /// The list is sorted by DESC by the Timestamp of the cheeps.
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <param name="pageNumber">The number of the page</param>
    /// <returns>A list of CheepDTOs</returns>
    /// <exception cref="ArgumentNullException">The name of the Author cannot be null</exception>
    /// <exception cref="ArgumentException">The page number cannot be below 1. The Author has to exist in the database</exception>
    public async Task<IEnumerable<CheepDTO>> GetPageOfCheepsByFollowed(string authorName, int pageNumber)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        int skipCount = (pageNumber - 1) * PageSize;

        List<CheepDTO> cheepList = await dbContext.Cheeps
            .Where(c => dbContext.Follows.Any(f => f.Follower == author && f.Following == c.Author))
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, c.TimeStamp, c.CheepId))
            .ToListAsync();

        return cheepList;
    }

    /// <summary>
    /// Returns a list of Cheeps (size: PageSize) sorted by time posted.
    /// </summary>
    /// <param name="pageNumber">The page number (starts at 1)</param>
    /// <returns></returns>
    public async Task<IEnumerable<CheepDTO>> GetPageOfCheeps(int pageNumber)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number cannot be under 1");

        int skipCount = (pageNumber - 1) * PageSize;

        List<CheepDTO> cheepList = await dbContext.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, c.TimeStamp, c.CheepId))
            .ToListAsync();

        return cheepList;
    }

    /// <summary>
    /// Returns the total amount of cheeps in the database
    /// </summary>
    /// <returns>an int count of the amount of cheep page</returns>
    public async Task<int> GetCheepPageAmountAll()
    {
        int count = await dbContext.Cheeps
            .CountAsync();

        int totalPages = count / PageSize;
        //Adds one extra page if the amount if cheeps is not perfectly divisible by the page size, where the remaining cheeps can be shown
        if (count % PageSize != 0)
        {
            totalPages++;
        }

        return totalPages;
    }

    /// <summary>
    /// Returns the amount of cheeps associated with a specific author
    /// </summary>
    /// <param name="authorName">The author whose cheeps are to be counted</param>
    /// <returns>an int count of the amount of cheep pages</returns>
    public async Task<int> GetCheepPageAmountAuthor(string authorName)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        int count = await dbContext.Cheeps
            .Where(c => c.AuthorId == author.AuthorId)
            .CountAsync();

        int totalPages = count / PageSize;
        //Adds one extra page if the amount if cheeps is not perfectly divisible by the page size, where the remaining cheeps can be shown
        if (count % PageSize != 0)
        {
            totalPages++;
        }

        return totalPages;
    }

    /// <summary>
    /// Returns the amount of cheeps associated with an author and who they follow
    /// </summary>
    /// <param name="authorName">The author whose cheeeps and cheeps from followed users should be counted</param>
    /// <returns>an int count of the amount of cheep pages</returns>
    public async Task<int> GetCheepPageAmountFollowed(string authorName)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        int count = await dbContext.Cheeps
            .Where(c => dbContext.Follows.Any(f => f.Follower == author && f.Following == c.Author))
            .CountAsync();

        int totalPages = count / PageSize;
        //Adds one extra page if the amount if cheeps is not perfectly divisible by the page size, where the remaining cheeps can be shown
        if (count % PageSize != 0)
        {
            totalPages++;
        }

        return totalPages;
    }


    /// <summary>
    /// Returns a list of cheep IDs from the cheeps written by the specified Author..
    /// </summary>
    /// <param name="authorName">The name of the Author</param>
    /// <returns>A IEnumerable of integers</returns>
    public async Task<IEnumerable<int>> GetCheepIDsByAuthor(string authorName)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        List<int> cheepList = await dbContext.Cheeps
            .Where(c => c.AuthorId == author.AuthorId)
            .Select(c => c.CheepId)
            .ToListAsync();
        return cheepList;
    }


    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="authorName">The name of the Author of the Cheep</param>
    /// <param name="text">The text in the Cheep</param>
    /// <param name="timestamp">The time the Cheep was posted</param>
    public async Task CreateCheep(string authorName, string text)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        if (text is null)
            throw new ArgumentNullException(nameof(text));
        if (text == "")
            throw new ArgumentException("Text can not be empty");
        if (text.Length > 160)
            throw new ArgumentException("Text can not be over 160 characters");

        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);

        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        DateTime timestamp =
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now.ToUniversalTime(), "Europe/Copenhagen");

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