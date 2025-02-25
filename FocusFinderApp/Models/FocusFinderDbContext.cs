using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Models;

public class FocusFinderDbContext : DbContext
{
    public FocusFinderDbContext(DbContextOptions<FocusFinderDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }
}