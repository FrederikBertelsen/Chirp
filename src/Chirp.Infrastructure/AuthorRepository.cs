using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public interface IAuthorRepository
{
    Task CreateAuthor(string authorName);
    Task RemoveAuthor(string authorName);
    Task<AuthorViewModel> FindAuthorByName(string authorName);
    Task<bool> DoesUserNameExists(string authorName);
}

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext dbContext;

    /// <summary>
    /// This Repository contains all direct communication with the database.
    /// We use this Repository to abstract hte data access layer from the rest of the application.
    /// </summary>
    /// <param name="dbContext">The ChirpDBContext that will be injected into this repo</param>
    public AuthorRepository(ChirpDBContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new author
    /// </summary>
    /// <param name="authorName">The name of the new author</param>
    /// <exception cref="ArgumentException">If an author with the authorName already exists</exception>
    public async Task CreateAuthor(string authorName)
    {
        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is not null)
            throw new ArgumentException($"Author with name '{authorName}' already exists");

        Author newAuthor = new Author() { Name = authorName};

        dbContext.Authors.Add(newAuthor);

        dbContext.Follows.Add(new Follow()
        {
            Follower = newAuthor, FollowerId = newAuthor.AuthorId, Following = newAuthor,
            FollowingId = newAuthor.AuthorId
        });

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes an Author and all of their Cheeps and Follows.
    /// </summary>
    /// <param name="authorName">The name of the Author</param>
    /// <exception cref="ArgumentException">If an author with authorName doesn't exist</exception>
    public async Task RemoveAuthor(string authorName)
    {
        if (authorName is null)
            throw new ArgumentNullException(nameof(authorName));
        Author? author = await dbContext.Authors
            .Where(a => a.Name == authorName)
            .Include(a => a.Cheeps)
            .SingleOrDefaultAsync();

        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' not found.");

        List<Follow> following = await dbContext.Follows.Where(f => f.FollowerId == author.AuthorId || f.FollowingId == author.AuthorId).ToListAsync();
        dbContext.Follows.RemoveRange(following);
        dbContext.Cheeps.RemoveRange(author.Cheeps);
        
        dbContext.Authors.Remove(author);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// finds an author by name
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>A AuthorViewModel containing the Author's</returns>
    /// <exception cref="ArgumentException">If an author with the authorName doesn't exist</exception>
    public async Task<AuthorViewModel> FindAuthorByName(string authorName)
    {
        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        if (author is null)
            throw new ArgumentException($"Author with name '{authorName}' doesn't exists");

        AuthorViewModel authorViewModel = new AuthorViewModel(author.Name);

        return authorViewModel;
    }

    /// <summary>
    ///checks if a author with the authorName exists
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>True if the author exists</returns>
    public async Task<bool> DoesUserNameExists(string authorName)
    {
        Author? author = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == authorName);
        return author is not null;
    }
}