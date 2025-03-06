using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace FocusFinderApp.Tests
{
    public class UsermanagementTests
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Set to false if you want to see the browser
            });
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Test]
        public async Task Register_ValidUser_ShouldSucceed()
        {
            await _page.GotoAsync("http://localhost:5240/Register");

            await _page.FillAsync("input[name='Username']", "testuser3");
            await _page.FillAsync("input[name='Email']", "test3@example.com");
            await _page.FillAsync("input[name='Password']", "Password@123");
            await _page.FillAsync("input[name='ConfirmPassword']", "Password@123");

            await _page.ClickAsync("input[type='submit']");

            Console.WriteLine("Current URL: " + _page.Url);
        
            // Wait a bit to let the redirection happen
            await _page.WaitForTimeoutAsync(5000);
        
            // Debugging: Print URL again after waiting
            Console.WriteLine("After waiting, URL: " + _page.Url);
        
            // Ensure we're redirected properly
            Assert.That(_page.Url, Does.Contain("/Locations"));

            // Assert redirection to homepage after successful registration
            await _page.WaitForURLAsync("http://localhost:5240/Locations");
            Assert.That(await _page.TitleAsync(), Does.Contain("Home Page"));
        }
    }
}