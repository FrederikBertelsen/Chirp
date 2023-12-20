namespace Chirp.Core;
public class Follow
{
    /// <summary>
    /// the author that is trying to follow
    /// </summary>
    public required Author Follower { get; set; }
    
    /// <summary>
    /// the id of the author that is trying to follow
    /// </summary>
    public required int FollowerId { get; set; }
    

/// <summary>
/// the author that is will be followed
/// </summary>
    public required Author Following { get; set; }

/// <summary>
/// the id of the author that will be followed
/// </summary>
    public required int FollowingId { get; set; }
}