using System.ComponentModel.DataAnnotations.Schema;
using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Cheep> Cheeps => Set<Cheep>();

    public DbSet<Follow> Follows => Set<Follow>();

    
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follow>()
            .HasKey(a => new { a.FollowerId, a.FollowingId });
        
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower )
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Following )
            .WithMany()
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}