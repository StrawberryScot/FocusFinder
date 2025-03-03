using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FocusFinderApp.Tests
{
    public static class TestDbContext
    {
        public static FocusFinderDbContext Create()
        {
            var options = new DbContextOptionsBuilder<FocusFinderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            return new FocusFinderDbContext(options);
        }
    }
}
