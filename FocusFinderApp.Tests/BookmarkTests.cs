using NUnit.Framework;
using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FocusFinderApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FocusFinderApp.Tests
{
    public class BookmarkTests
    {
        private FocusFinderDbContext _dbContext;

        private BookmarkController _controller;

[SetUp]
public void Setup()
{
    var options = new DbContextOptionsBuilder<FocusFinderDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

    _dbContext = new FocusFinderDbContext(options);
    _dbContext.Database.EnsureCreated();

    // Mocking session
    var httpContext = new DefaultHttpContext();
    var sessionMock = new Mock<ISession>(); // Use Moq for mocking
    var sessionValues = new Dictionary<string, byte[]>(); // Store session values

    sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
        .Callback<string, byte[]>((key, value) => sessionValues[key] = value);

    sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
        .Returns((string key, out byte[] value) => sessionValues.TryGetValue(key, out value));

    httpContext.Session = sessionMock.Object; // Assign mock session

    // Set UserId in session before each test
    var userIdBytes = BitConverter.GetBytes(1);  // Example for setting userId in session
    httpContext.Session.Set("UserId", userIdBytes);

    _controller = new BookmarkController(_dbContext)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        }
    };
}


//         [SetUp] // Runs before each test
// public void Setup()
// {
//     var options = new DbContextOptionsBuilder<FocusFinderDbContext>()
//         .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique per test session
//         .Options;

//     _dbContext = new FocusFinderDbContext(options);
//     _dbContext.Database.EnsureCreated();

//     // Mocking session
//     var httpContext = new DefaultHttpContext();
//     var sessionMock = new Mock<ISession>(); // Use Moq for mocking
//     var sessionValues = new Dictionary<string, byte[]>(); // Store session values

//     sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
//         .Callback<string, byte[]>((key, value) => sessionValues[key] = value);

//     sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
//         .Returns((string key, out byte[] value) => sessionValues.TryGetValue(key, out value));

//     httpContext.Session = sessionMock.Object; // Assign mock session

//     // Mock IHttpContextAccessor
//     var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//     httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

//     // Set UserId in session
//     var userIdBytes = BitConverter.GetBytes(1);
//     httpContext.Session.Set("UserId", userIdBytes);

//     _controller = new BookmarkController(_dbContext)
//     {
//         ControllerContext = new ControllerContext
//         {
//             HttpContext = httpContext
//         }
//     };
// }


        // [SetUp] // Runs before each test
        // public void Setup()
        // {
        //     var options = new DbContextOptionsBuilder<FocusFinderDbContext>()
        //         .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique per test session
        //         .Options;

        //     _dbContext = new FocusFinderDbContext(options);
        //     _dbContext.Database.EnsureCreated();

        //     // Mocking session
        //     var httpContext = new DefaultHttpContext();
        //     var sessionMock = new Mock<ISession>(); // Use Moq for mocking
        //     var sessionValues = new Dictionary<string, byte[]>(); // Store session values

        //     sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
        //         .Callback<string, byte[]>((key, value) => sessionValues[key] = value);
    
        //     sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
        //         .Returns((string key, out byte[] value) => sessionValues.TryGetValue(key, out value));

        //     httpContext.Session = sessionMock.Object; // Assign mock session

        //     // Set UserId in session
        //     var userIdBytes = BitConverter.GetBytes(1);
        //     httpContext.Session.Set("UserId", userIdBytes);

        //     _controller = new BookmarkController(_dbContext)
        //     {
        //         ControllerContext = new ControllerContext { HttpContext = httpContext }
        //     };
        // }


        [TearDown] // Runs after each test
        public void TearDown()
        {
            _dbContext.Bookmarks.RemoveRange(_dbContext.Bookmarks);
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();

            _controller?.Dispose();
            _dbContext?.Dispose();
            // _dbContext?.Dispose();
            // _controller?.Dispose();
            // _controller = null;
        }

        [Test]
        public void Test_BookmarkAddedToDatabase()
        {
            // Arrange
            var bookmark = new Bookmark { userId = 1, locationId = 1 };

            // Act
            _dbContext.Bookmarks.Add(bookmark);
            _dbContext.SaveChanges();

            // Assert
            var savedBookmark = _dbContext.Bookmarks.FirstOrDefault(a => a.userId == 1);
            Assert.That(savedBookmark, Is.Not.Null, "Achievement should be saved to the database.");
            Assert.That(savedBookmark.userId, Is.EqualTo(1), "User ID should be 1.");
        }
        
        
        [Test]
        public void Test_BookmarkAddedToUserBookmarkPage()
        {
            // Arrange
            var user = new User 
            {  
                Id = 1, 
                Username = "testUser",
                Email = "test@example.com",
                Password = "hashedpassword123"
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var bookmark = new Bookmark { userId = 1, locationId = 1 };
            _dbContext.Bookmarks.Add(bookmark);
            _dbContext.SaveChanges();

            // Setup session manually (instead of mocking GetString/GetInt32)
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetString("Username", "testUser");

            _controller = new BookmarkController(_dbContext) 
            { 
                ControllerContext = new ControllerContext { HttpContext = httpContext } 
            };

            // Act
            var result = _controller.UserBookmarks();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Bookmark>;

            Assert.That(model, Is.Not.Null, "Expected model to not be null");
            Assert.That(model.Count, Is.EqualTo(1), "Expected 1 bookmark but found 0");
        }

            
        [Test]
        public void Test_BookMarkRemovedDatabase()
        {
            // Arrange
            var bookmark = new Bookmark { userId = 1, locationId = 1 };

            // Act
            _dbContext.Bookmarks.Add(bookmark);
            _dbContext.SaveChanges();
            _dbContext.Bookmarks.Remove(bookmark);
            _dbContext.SaveChanges();

            // Assert
            var savedBookmark = _dbContext.Bookmarks.FirstOrDefault(a => a.userId == 1);
            Assert.That(savedBookmark, Is.Null, "There should be no bookmark in database");
            if (savedBookmark != null)
            {
                Assert.That(savedBookmark.userId, Is.Not.EqualTo(1), "User ID should not be 1.");
            }
        }
        [Test]
        public void Test_BookmarkRemovedFromUserBookmarkPage()
        {
            // Arrange
            var user = new User 
            {  
                Id = 1, 
                Username = "testUser",
                Email = "test@example.com",
                Password = "hashedpassword123"
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var bookmark = new Bookmark { userId = 1, locationId = 1 };
            _dbContext.Bookmarks.Add(bookmark);
            _dbContext.SaveChanges();
            _dbContext.Bookmarks.Remove(bookmark);
            _dbContext.SaveChanges();

            // Setup session manually (instead of mocking GetString/GetInt32)
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetString("Username", "testUser");

            _controller = new BookmarkController(_dbContext) 
            { 
                ControllerContext = new ControllerContext { HttpContext = httpContext } 
            };

            // Act
            var result = _controller.UserBookmarks();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Bookmark>;

            Assert.That(model, Is.Empty, "Expected model to not be null");
            if (model != null)
            {
                Assert.That(model.Count, Is.Not.EqualTo(1), "Expected 1 bookmark but found 0");
            }
        }
//         [Test]
// public void Test_UserBookmarks_EmptyBookmarks()
// {
//     // Arrange
//     var user = new User 
//     {  
//         Id = 1, 
//         Username = "testUser",
//         Email = "test@example.com",
//         Password = "hashedpassword123"
//     };
//     _dbContext.Users.Add(user);
//     _dbContext.SaveChanges();

//     var httpContext = new DefaultHttpContext();
//     httpContext.Session.SetInt32("UserId", 1); // Make sure UserId is set
//     _controller = new BookmarkController(_dbContext) 
//     { 
//         ControllerContext = new ControllerContext { HttpContext = httpContext }
//     };

//     // Act
//     var result = _controller.UserBookmarks();

//     // Assert
//     var viewResult = result as ViewResult;
//     var model = viewResult?.Model as List<Bookmark>;
//     Assert.That(model, Is.Empty, "Expected model to be empty as user has no bookmarks.");
// }

        
        
        // [Test]
        // public void Test_UserBookmarks_EmptyBookmarks()
        // {
        //     // Arrange
        //     var user = new User 
        //     {  
        //         Id = 1, 
        //         Username = "testUser",
        //         Email = "test@example.com",
        //         Password = "hashedpassword123"
        //     };
        //     _dbContext.Users.Add(user);
        //     _dbContext.SaveChanges();

        //     var httpContext = new DefaultHttpContext();
        //     httpContext.Session.SetInt32("UserId", 1);
        //     _controller = new BookmarkController(_dbContext) 
        //     { 
        //         ControllerContext = new ControllerContext { HttpContext = httpContext }
        //     };

        //     // Act
        //     var result = _controller.UserBookmarks();

        //     // Assert
        //     var viewResult = result as ViewResult;
        //     var model = viewResult?.Model as List<Bookmark>;
        //     Assert.That(model, Is.Empty, "Expected model to be empty as user has no bookmarks.");
        // }




        // [Test]
        // public void Test_AddBookmark_RedirectToCorrectUrl()
        // {
        //     // Arrange
        // var httpContext = new DefaultHttpContext();
        // httpContext.Session.SetString("Username", "testUser");

        //     _controller = new BookmarkController(_dbContext)
        //     {
        //         ControllerContext = new ControllerContext { HttpContext = httpContext }
        //     };

        //     // Act
        //     var result = _controller.Add(1, "/Bookmarks");

        //     // Assert - Check if the redirect URL is correct
        //     var redirectResult = result as RedirectResult;
        //     Assert.That(redirectResult, Is.Not.Null, "Expected redirect result.");
        //     Assert.That(redirectResult?.Url, Is.EqualTo("/Bookmarks"), "Expected redirect URL to be /Bookmarks.");
        // }
    }
    
}

public class MockHttpSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

    public bool IsAvailable => true;

    public string Id => "mock-session-id";

    public void Clear() => _sessionStorage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        // Mock committing session
        return Task.CompletedTask;
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        // Mock loading session
        return Task.CompletedTask;
    }

    public void Remove(string key) => _sessionStorage.Remove(key);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

    // Implement the Keys property
    public IEnumerable<string> Keys => _sessionStorage.Keys;
}
