
using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using Bogus;
using System.Threading.Tasks;




namespace FocusFinderApp.Tests
{
    public class UsermanagementTests : PageTest
    {
        private Faker _faker;
        private string testPassword = "Password@123";

        [SetUp]
        public void Setup()
        {
            _faker = new Faker();
        }

        // Helper methods

        private async Task CreateAccount(string username, string email, string password, string confirmPassword = "")
        {
            await Page.GotoAsync("http://localhost:5240/Register");
            await Page.FillAsync($"input[name='Username']", username);
            await Page.FillAsync($"input[name='Email']", email);
            await Page.FillAsync($"input[name='Password']", password);
            await Page.FillAsync($"input[name='ConfirmPassword']", confirmPassword);
            await Page.ClickAsync("input[type='submit']");
        }
    

       [Test]
        public async Task Register_ValidUser_ShouldSucceed()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            // Assert redirection to homepage after successful registration
            Assert.That(await Page.TitleAsync(), Does.Contain("Home Page"));
        }

        [Test]
        public async Task Register_ExistingUserName_ShouldFail()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            // Attempt to create account with same username and email
            await CreateAccount(username, email, password, confirmPassword);

            // Assert that the registration page is still displayed
            Assert.That(Page.Url, Does.Contain("Register"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("Email is already taken").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Register_ExistingUserEmail_ShouldFail()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            // Attempt to create account with same username and email
            await CreateAccount(username, email, password, confirmPassword);

            // Assert that the registration page is still displayed
            Assert.That(Page.Url, Does.Contain("Register"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("Username is already taken").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Register_InvalidPassword_ShouldFail()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = "password";
            string confirmPassword = "password";

            await CreateAccount(username, email, password, confirmPassword);

            // Assert that the registration page is still displayed
            Assert.That(Page.Url, Does.Contain("Register"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Register_Passwords_Dont_Match_ShouldFail()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = "password";

            await CreateAccount(username, email, password, confirmPassword);

            // Assert that the registration page is still displayed
            Assert.That(Page.Url, Does.Contain("Register"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("Passwords do not match").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Register_Empty_Fields_Required_Message()
        {
            string username = "";
            string email = "";
            string password = "";
            string confirmPassword = "";

            await CreateAccount(username, email, password, confirmPassword);

            // Assert that the registration page is still displayed
            Assert.That(Page.Url, Does.Contain("Register"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("required").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }






    }
}
