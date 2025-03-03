using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Models;

public class FocusFinderDbContext : DbContext
{
    public FocusFinderDbContext(DbContextOptions<FocusFinderDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }
    public DbSet<Location>? Locations { get; set; }
    public DbSet<Review>? Reviews { get; set; }
    public DbSet<Bookmark>? Bookmarks { get; set; }
    public DbSet<Visit>? Visits { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<Visit>()
            .HasOne(v => v.User)  // A Visit has one User
            .WithMany(u => u.Visits)  // A User has many Visits
            .HasForeignKey(v => v.userId) // Foreign key
            .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete their visits

        modelBuilder.Entity<Visit>()
            .HasOne(v => v.Location) // A Visit has one Location
            .WithMany()  // A Location can have many visits 
            .HasForeignKey(v => v.locationId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.userId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.Location)
            .WithMany() // many bookmarks can exist for a location
            .HasForeignKey(b => b.locationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

