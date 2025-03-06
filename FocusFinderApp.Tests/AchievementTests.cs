using NUnit.Framework;
using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FocusFinderApp.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FocusFinderApp.Tests
{
    public class AchievementTests
    {
        private FocusFinderDbContext _dbContext;

        private BookmarkController _controller;

        [SetUp] // Runs before each test
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FocusFinderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique per test session
                .Options;

            _dbContext = new FocusFinderDbContext(options);
            _dbContext.Database.EnsureCreated();
            _controller = new BookmarkController(_dbContext);
        }

        [TearDown] // Runs after each test
        public void TearDown()
        {
            _controller.Dispose();
            _dbContext.Dispose();
        }

        [Test]
        public void Test_VisitAchievement_AddsAchievementToDatabase()
        {
            // Arrange
            var achievement = new Achievement { userId = 1, visits = 0 };

            // Act
            _dbContext.Achievements.Add(achievement);
            _dbContext.SaveChanges();

            // Simulate a visit update
            Achievement.UpdateUserAchievements(_dbContext, 1, "visit");

            // Assert
            var savedAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
            Assert.That(savedAchievement, Is.Not.Null, "Achievement should be saved to the database.");
            Assert.That(savedAchievement.visits, Is.EqualTo(1), "Visits count should be 1.");
        }
        [Test]
        public void Test_ReviewAchievement_AddsAchievementToDatabase()
        {
            // Arrange
            var achievement = new Achievement { userId = 1, reviewsLeft = 0 };

            // Act
            _dbContext.Achievements.Add(achievement);
            _dbContext.SaveChanges();

            // Simulate a review update
            Achievement.UpdateUserAchievements(_dbContext, 1, "review");

            // Assert
            var savedAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
            Assert.That(savedAchievement, Is.Not.Null, "Achievement should be saved to the database.");
            Assert.That(savedAchievement.reviewsLeft, Is.EqualTo(1), "Review count should be 1.");
        }
        [Test]
        public void Test_BookmarkAchievement_AddsAchievementToDatabase()
        {
            // Arrange
            var achievement = new Achievement { userId = 1, bookmarks = 0 };

            // Act
            _dbContext.Achievements.Add(achievement);
            _dbContext.SaveChanges();

            // Simulate a review update
            Achievement.UpdateUserAchievements(_dbContext, 1, "bookmark");

            // Assert
            var savedAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
            Assert.That(savedAchievement, Is.Not.Null, "Achievement should be saved to the database.");
            Assert.That(savedAchievement.bookmarks, Is.EqualTo(1), "Review count should be 1.");
        }
        [Test]
        public void Test_NewCityAchievement_AddsAchievementToDatabase()
        {
            // Arrange
            var achievement = new Achievement { userId = 1, citiesVisited = 0 };

            // Act
            _dbContext.Achievements.Add(achievement);
            _dbContext.SaveChanges();

            // Simulate a review update
            Achievement.UpdateUserAchievements(_dbContext, 1, "newCity");

            // Assert
            var savedAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
            Assert.That(savedAchievement, Is.Not.Null, "Achievement should be saved to the database.");
            Assert.That(savedAchievement.citiesVisited, Is.EqualTo(1), "Review count should be 1.");
        }
            [Test]
        public void Test_Incremenent_Visit_Count()
        {
            // Arrange
            var achievement = new Achievement { userId = 1, visits = 0 };

            // Act
            _dbContext.Achievements.Add(achievement);
            _dbContext.SaveChanges();

            // Simulate a visit update
            Achievement.UpdateUserAchievements(_dbContext, 1, "visit");
            var existingAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
            existingAchievement.visits += 1;
            _dbContext.SaveChanges();

            // Assert
            
            var updatedAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
            Assert.That(updatedAchievement, Is.Not.Null, "Achievement should be saved to the database.");
            Assert.That(updatedAchievement.visits, Is.EqualTo(2), "Visits count should be 2.");
        }
        // [Test]
        // public void Test_Increment_Visit_Count()
        // {
        //     // Arrange
        //     var achievement = new Achievement { userId = 1, visits = 1 };
        //     _dbContext.Achievements.Add(achievement);
        //     _dbContext.SaveChanges();

        //     // Act
        //     var existingAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
        //     existingAchievement.visits += 1;
        //     _dbContext.SaveChanges();

        //     // Assert
        //     var updatedAchievement = _dbContext.Achievements.FirstOrDefault(a => a.userId == 1);
        //     Assert.AreEqual(2, updatedAchievement.visits, "Visit count should increment by 1.");
        // }
    }

    // Mock session class
public class MockHttpSession : ISession
{
    private readonly Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();

    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public string Id => Guid.NewGuid().ToString();

    public bool IsAvailable => true;

    public void Clear() => _sessionStorage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _sessionStorage.Remove(key);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public bool TryGetValue(string key, out byte[] value)
    {
        if (_sessionStorage.TryGetValue(key, out var objValue) && objValue is byte[] byteValue)
        {
            value = byteValue;
            return true;
        }
        value = null;
        return false;
    }

    public void SetInt32(string key, int value) => _sessionStorage[key] = BitConverter.GetBytes(value);

    public int? GetInt32(string key)
    {
        if (_sessionStorage.TryGetValue(key, out var objValue) && objValue is byte[] byteValue)
        {
            return BitConverter.ToInt32(byteValue, 0);
        }
        return null;
    }

    public void SetString(string key, string value) => _sessionStorage[key] = System.Text.Encoding.UTF8.GetBytes(value);

    public string GetString(string key)
    {
        if (_sessionStorage.TryGetValue(key, out var objValue) && objValue is byte[] byteValue)
        {
            return System.Text.Encoding.UTF8.GetString(byteValue);
        }
        return null;
    }
}
}
