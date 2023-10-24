using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Cheep> Cheeps => Set<Cheep>();

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}