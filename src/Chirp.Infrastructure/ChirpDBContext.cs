using System.ComponentModel.DataAnnotations.Schema;
using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Cheep> Cheeps => Set<Cheep>();

    public DbSet<Follow> Follows => Set<Follow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follow>()
            .HasKey(a => new { a.Follower, a.Following });
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower }.HasMa;
        modelBuilder.Entity<Follow>()
            .HasKey(a => new { a.Follower, a.Following });
    }
}