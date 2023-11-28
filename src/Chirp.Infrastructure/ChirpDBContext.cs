using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Cheep> Cheeps => Set<Cheep>();

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .HasKey(a => a.AuthorId)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Cheep>()
                .HasKey(a => a.CheepId)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);
        }
        */
}