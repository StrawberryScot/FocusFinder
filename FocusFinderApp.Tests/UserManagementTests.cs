
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

        
    

       [Test]
        public async Task Register_ValidUser_ShouldSucceed()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();

            await Page.GotoAsync("http://localhost:5240/Register");

            await Page.FillAsync($"input[name='Username']", username);
            await Page.FillAsync($"input[name='Email']", email);
            await Page.FillAsync($"input[name='Password']", testPassword);
            await Page.FillAsync($"input[name='ConfirmPassword']", testPassword);

            await Page.ClickAsync("input[type='submit']");

            Console.WriteLine("Current URL: " + Page.Url);

            // Wait for navigation after form submission
            await Page.WaitForURLAsync("http://localhost:5240/Locations");

            // Debugging: Print URL again after waiting
            Console.WriteLine("After waiting, URL: " + Page.Url);

            // Ensure we're redirected properly
            Assert.That(Page.Url, Does.Contain("/Locations"));

            // Assert redirection to homepage after successful registration
            Assert.That(await Page.TitleAsync(), Does.Contain("Home Page"));
        }
    }
}
