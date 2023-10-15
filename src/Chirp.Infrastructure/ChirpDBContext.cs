using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

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