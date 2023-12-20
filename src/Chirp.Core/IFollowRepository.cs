namespace Chirp.Core;

public interface IFollowRepository
{
    Task AddFollower(string followerName, string followingName);

    Task RemoveFollower(string followerName, string followingName);

    Task<bool> IsFollowing(string followerName, string followingName);

    Task<IEnumerable<FollowDTO>> GetFollowing(string authorName);
}