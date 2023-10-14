using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }

    public string DbPath { get; }

    public ChirpDBContext()
    {
        string dbFileName = "Chirp.db";
        string folderPath = Path.GetFullPath(".");
        DbPath = Path.Combine(folderPath, dbFileName);
        // Console.WriteLine($"Saved SQLite DB at: {DbPath}");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }

    public List<Cheep> Cheeps { get; set; } = new();
}

public class Cheep
{
    public int CheepId { get; set; }
    public required Author Author { get; set; }
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }
}