
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

        private async Task Login(string email, string password)
        {
            await Page.GotoAsync("http://localhost:5240/Login");
            await Page.FillAsync($"input[name='email']", email);
            await Page.FillAsync($"input[name='password']", password);
            await Page.ClickAsync("input[type='submit']");
        }

        private async Task Logout()
        {
            await Page.GotoAsync("http://localhost:5240/Locations");
            await Page.ClickAsync("text=Logout");
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

        [Test]
        public async Task Login_ValidUser_ShouldSucceed()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, password);

            // Assert redirection to homepage after successful login
            Assert.That(await Page.TitleAsync(), Does.Contain("Home Page"));
        }

        [Test]
        public async Task Login_InvalidUser_ShouldFail()
        {
            string email = _faker.Internet.Email();
            string password = testPassword;

            await Login(email, password);

            // Assert that the login page is still displayed
            Assert.That(Page.Url, Does.Contain("Login"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("not found").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Login_Empty_Fields_Required_Message()
        {
            string email = "";
            string password = "";

            await Login(email, password);

            // Assert that the login page is still displayed
            Assert.That(Page.Url, Does.Contain("Login"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("required").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Login_Wrong_Password_Invalid_Password_Message()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;
            string wrongPassword = "wrongpassword";

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, wrongPassword);

            // Assert that the login page is still displayed
            Assert.That(Page.Url, Does.Contain("Login"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("Invalid").IgnoreCase
                            .Or.Contains("error").IgnoreCase
                            .Or.Contains("failed").IgnoreCase);
        }

        [Test]
        public async Task Logout_ShouldSucceed()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, password);

            await Logout();

            string pageText = await Page.InnerTextAsync("body");

            // Assert redirection to homepage after successful logout
            Assert.That(pageText, Does.Contain("Register").IgnoreCase);
        }

        [Test]
        public async Task Login_Session_Works_Profile_Button_Appears()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, password);

            string pageText = await Page.InnerTextAsync("body");

            // Assert that the profile button appears after successful login

            Assert.That(pageText, Does.Contain("Profile").IgnoreCase);
        }

        [Test]
        public async Task Profile_Page_Exists()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, password);

            await Page.GotoAsync("http://localhost:5240/Profile/" + username);

            // Assert that the profile page is displayed
            Assert.That(await Page.TitleAsync(), Does.Contain("Profile"));
        }

        [Test]
        public async Task Profile_Page_Invalid_User_Returns_Not_Found()
        {
            await Page.GotoAsync("http://localhost:5240/Profile/nonexistentuser");

            string pageText = await Page.InnerTextAsync("body");

            // Assert that the profile page is displayed
            Assert.That(pageText, Does.Contain("Not Found").IgnoreCase);
        }

        [Test]
        public async Task Edit_Profile_Page_Exists()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, password);

            await Page.GotoAsync("http://localhost:5240/Profile/Edit");

            // Assert that the edit profile page is displayed
            Assert.That(await Page.TitleAsync(), Does.Contain("Edit Profile"));
        }

        [Test]
        public async Task Edit_Profile_Page_Invalid_User_Returns_Not_Found()
        {
            await Page.GotoAsync("http://localhost:5240/Profile/Edit");

            Assert.That(Page.Url, Does.Contain("Login"));

            string pageText = await Page.InnerTextAsync("body");

            Assert.That(pageText, Does.Contain("You must be logged in to edit your profile").IgnoreCase);    
        }

        [Test]
        public async Task EditProfile_ShouldSucceed()
        {
            string username = _faker.Internet.UserName();
            string email = _faker.Internet.Email();
            string password = testPassword;
            string confirmPassword = testPassword;

            await CreateAccount(username, email, password, confirmPassword);

            await Login(email, password);

            await Page.GotoAsync("http://localhost:5240/Profile/Edit");

            string FirstName = "John";
            string LastName = "Doe";

            await Page.FillAsync($"input[name='FirstName']", FirstName);
            await Page.FillAsync($"input[name='LastName']", LastName);

            //  Debugging: Print input values before clicking submit
            string filledFirstName = await Page.InputValueAsync("input[name='FirstName']");
            string filledLastName = await Page.InputValueAsync("input[name='LastName']");

            Console.WriteLine($"Before submitting - FirstName: {filledFirstName}, LastName: {filledLastName}");
            await Page.ClickAsync("form[action='/Profile/Edit'] button[type='submit']");


            await Page.WaitForTimeoutAsync(2000); // Temporary delay to let things process

            //  Print the body text right after clicking submit
            string postSubmitText = await Page.InnerTextAsync("body");
            Console.WriteLine("AFTER SUBMIT: " + postSubmitText);


            Assert.That(await Page.TitleAsync(), Does.Contain("Profile"));

            await Page.GotoAsync("http://localhost:5240/Profile/" + username);

            string pageText = await Page.InnerTextAsync("body");

            Console.WriteLine(pageText);

            Assert.That(pageText, Does.Contain(FirstName).IgnoreCase
                            .And.Contains(LastName).IgnoreCase);
        }
    }
}
