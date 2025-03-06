using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FocusFinderApp.Controllers;
using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net.Http;

namespace FocusFinderApp.Tests
{
    public class UsersControllerTests
    {
        private FocusFinderDbContext _dbContext;
        private Mock<ILogger<UsersController>> _loggerMock;
        private UsersController _controller;
        private IServiceProvider _serviceProvider;

        [SetUp] // Runs before each test
        public void Setup()
        {
            _dbContext = TestDbContext.Create();
            _loggerMock = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_dbContext, _loggerMock.Object);

            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            _serviceProvider = services.BuildServiceProvider();

            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            };
            _controller.Url = new UrlHelper(actionContext);
        }

        [TearDown] // Cleanup after each test
        public void TearDown()
        {
            _controller.Dispose();
            _dbContext.Dispose();
            if (_serviceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
        public class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

            public IEnumerable<string> Keys => _sessionStorage.Keys;

            public bool IsAvailable => true;

            public string Id => Guid.NewGuid().ToString();

            public void Clear() => _sessionStorage.Clear();

            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public void Remove(string key) => _sessionStorage.Remove(key);

            public void Set(string key, byte[] value) => _sessionStorage[key] = value;

            public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
        }

        [Test]
        public void Register_Get_ReturnsView_1()
        {
            var result = _controller.Register();
            Assert.That(result, Is.TypeOf<ViewResult>());
        }

        [Test]
        public void Register_Post_ValidUser_RedirectsToHome_2()
        {
            // Arrange
            var newUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };

            // Act
            var result = _controller.Register(newUser);

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            var actionResult = (RedirectToActionResult)result;
            Assert.That(actionResult.ActionName, Is.EqualTo("Index"));
            Assert.That(actionResult.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public void Register_Post_MissingFields_ReturnsValidationErrors_3()
        {
            var newUser = new User(); // Empty user (missing fields)

            var result = _controller.Register(newUser);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData.ModelState.IsValid, Is.False);
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Username"), Is.True);
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Email"), Is.True);
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Password"), Is.True);
        }


        [Test]
        public void Register_Post_DuplicateUsername_ReturnsError_4()
        {
            var existingUser = new User
            {
                Username = "existinguser",
                Email = "test@example.com",
                Password = "Password@123"
            };
            _dbContext.Users?.Add(existingUser);
            _dbContext.SaveChanges(); // Simulate existing user

            var newUser = new User
            {
                Username = "existinguser", // Duplicate username
                Email = "newemail@example.com",
                Password = "Password@123"
            };

            var result = _controller.Register(newUser);

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Username"), Is.True);
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Username"), Is.True);
            var usernameErrors = viewResult.ViewData.ModelState["Username"]?.Errors;
            Assert.That(usernameErrors, Is.Not.Null);
            Assert.That(usernameErrors[0].ErrorMessage, Is.EqualTo("Username is already taken"));
        }
        [Test]
        public void Register_Post_DuplicateEmail_ReturnsError_5()
        {
            var existingUser = new User
            {
                Username = "existinguser",
                Email = "test@example.com",
                Password = "Password@123"
            };
            _dbContext.Users?.Add(existingUser);
            _dbContext.SaveChanges(); 

            var newUser = new User
            {
                Username = "newuser",
                Email = "test@example.com", //duplicate email
                Password = "Password@123"
            };

            var result = _controller.Register(newUser);
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Email"), Is.True);
            Assert.That(viewResult.ViewData.ModelState.ContainsKey("Email"), Is.True);
            var emailErrors = viewResult.ViewData.ModelState["Email"]?.Errors;
            Assert.That(emailErrors, Is.Not.Null);
            Assert.That(emailErrors[0].ErrorMessage, Is.EqualTo("Email is already taken"));
        }
        [Test]
        public void Register_Post_Stores_HashedPassword_6()
        {
            var newUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };

            var result = _controller.Register(newUser);
            var storedUser = _dbContext.Users?.FirstOrDefault(u => u.Username == "testuser");

            Assert.That(storedUser, Is.Not.Null);
            Assert.That(storedUser.Password, Is.Not.EqualTo("Password@123")); // Should be hashed
        }
        [Test]
        public void Register_Post_AddsUserToDatabase_7()
        {
            var newUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };

            var result = _controller.Register(newUser);
            var savedUser = _dbContext.Users?.FirstOrDefault(u => u.Username == "testuser");

            Assert.That(savedUser, Is.Not.Null);
            Assert.That(savedUser.Email, Is.EqualTo("test@example.com"));
        }
        [Test]
        public void Profile_Get_ValidUser_ReturnsView_8()
        {
            var existingUser = new User { 
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };
            _dbContext.Users?.Add(existingUser);
            _dbContext.SaveChanges();

            var result = _controller.Profile("testuser");

            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.EqualTo(existingUser));
        }

        [Test]
        public void Profile_Get_InvalidUser_ReturnsNotFound_9()
        {
            var result = _controller.Profile("nonexistentuser");

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        

        [Test]
        public void Edit_Get_LoggedInUser_ReturnsView_10()
        {
            var existingUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };
            _dbContext.Users?.Add(existingUser);
            _dbContext.SaveChanges();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new TestSession();
            _controller.ControllerContext.HttpContext.Session.SetString("Username", "testuser");

            var result = _controller.Edit("testuser");

            Assert.That(result, Is.TypeOf<ViewResult>());
        }
           
    }
}
    

