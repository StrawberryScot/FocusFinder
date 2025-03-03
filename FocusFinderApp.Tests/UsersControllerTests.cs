using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FocusFinderApp.Controllers;
using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FocusFinderApp.Tests
{
    public class UsersControllerTests
    {
        private FocusFinderDbContext _dbContext;
        private Mock<ILogger<UsersController>> _loggerMock;
        private UsersController _controller;

        [SetUp] // Runs before each test
        public void Setup()
        {
            _dbContext = TestDbContext.Create();
            _loggerMock = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_dbContext, _loggerMock.Object);
        }

        [TearDown] // Cleanup after each test
        public void TearDown()
        {
            _controller.Dispose(); 
            _dbContext.Dispose();  
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
    }
}
    

