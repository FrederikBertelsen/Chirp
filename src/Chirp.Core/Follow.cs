using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core;
public class Follow
{
    [ForeignKey(nameof(Author))]
    public required Author Follower { get; set; }
    
    [ForeignKey(nameof(Author))]
    public required Author Following { get; set; }
}