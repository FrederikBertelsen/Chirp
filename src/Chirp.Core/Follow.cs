using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core;
public class Follow
{
    public required Author Follower { get; set; }
    public required int FollowerId { get; set; }

    public required Author Following { get; set; }
    public required int FollowingId { get; set; }
}