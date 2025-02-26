using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Models;

public class FocusFinderDbContext : DbContext
{
    public FocusFinderDbContext(DbContextOptions<FocusFinderDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }



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

