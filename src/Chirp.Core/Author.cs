namespace Chirp.Core;
using System.ComponentModel.DataAnnotations; // for [Key]
using System.ComponentModel.DataAnnotations.Schema; // for [DatabaseGenerated]

public class Author
{
    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }

    public List<Cheep> Cheeps { get; set; } = new();
}