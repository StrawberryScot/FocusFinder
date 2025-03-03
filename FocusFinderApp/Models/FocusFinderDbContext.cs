using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Models;

public class FocusFinderDbContext : DbContext
{
    public FocusFinderDbContext(DbContextOptions<FocusFinderDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }
    public DbSet<Location>? Locations { get; set; }
    public DbSet<Review>? Reviews { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Visit>? Visits { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}

