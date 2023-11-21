using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public interface IFollowRepository
{
    Task AddFollower(string followerName, string followingName);
    
    Task RemoveFollower(string followerName, string followingName);
    
    Task<bool> IsFollowing(string followerName, string followingName);
    
   
   
}

public class FollowRepository : IFollowRepository
{
    private readonly ChirpDBContext dbContext;

    /// <summary>
    /// This Repository contains all direct communication with the database.
    /// We use this Repository to abstract hte data access layer from the rest of the application.
    /// </summary>
    /// <param name="dbContext">The ChirpDBContext that will be injected into this repo</param>
    public FollowRepository(ChirpDBContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddFollower(string followerName, string followingName)
    {
        if (followerName is null)
            throw new ArgumentNullException(nameof(followerName));
        if (followingName is null)
            throw new ArgumentNullException(nameof(followingName));

        Author? followerAuthor = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == followerName);
        Author? followingAuthor = await dbContext.Authors.SingleOrDefaultAsync(a => a.Name == followingName);

        if (followerAuthor is null)
            throw new ArgumentException($"Author with name '{followerName}' not found.");
        
        if (followingAuthor is null)
            throw new ArgumentException($"Author with name '{followingName}' not found.");
        
        Follow newFollow = new Follow() { Follower = followerAuthor, Following = followingAuthor};

        dbContext.Follows.Add(newFollow);
        await dbContext.SaveChangesAsync();

    }

    public async Task RemoveFollower(string followerName, string followingName)
    {
        if (followerName is null)
            throw new ArgumentNullException(nameof(followerName));
        if (followingName is null)
            throw new ArgumentNullException(nameof(followingName));
        
        Follow? follow = await dbContext.Follows
            .Include(f => f.Follower)
            .Include(f=> f.Following)
            .SingleOrDefaultAsync(f => f.Follower.Name == followerName && f.Following.Name == followingName);

        if (follow is null)
            throw new ArgumentException($"follow between '{followerName}' -> '{followingName}' not found.");

        dbContext.Follows.Remove(follow);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsFollowing(string followerName, string followingName)
    {
        if (followerName is null)
            throw new ArgumentNullException(nameof(followerName));
        if (followingName is null)
            throw new ArgumentNullException(nameof(followingName));

        Follow? follow = await GetFollow(followerName, followingName);
        
        return follow is not null;
    }

    private async Task<Follow?> GetFollow(string followerName, string followingName)
    {
        Follow? follow = await dbContext.Follows
            .Include(f => f.Follower)
            .Include(f=> f.Following)
            .SingleOrDefaultAsync(f => f.Follower.Name == followerName && f.Following.Name == followingName);
        return follow;
    }
    
}