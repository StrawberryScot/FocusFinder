using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FocusFinderApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Controllers;

public class SessionController : Controller
{
    private readonly FocusFinderDbContext _dbContext;
    private readonly ILogger<SessionController> _logger;

    public SessionController(FocusFinderDbContext dbContext, ILogger<SessionController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Route("/Login")]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [Route("/Login")]
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            TempData["Error"] = "Email and password are required";
            return RedirectToAction("Login");
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Email not found";
            return RedirectToAction("Login");
        }

        var passwordHasher = new PasswordHasher<User>();
        var result = user.Password != null ? passwordHasher.VerifyHashedPassword(user, user.Password, password) : PasswordVerificationResult.Failed;

        if (result == PasswordVerificationResult.Success)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["ErrorMessage"] = "Invalid password";
            return RedirectToAction("Login");
        }
    }
   
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [Route("/Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        Response.Cookies.Delete("AspNetCore.Cookies");
        return RedirectToAction("Index", "Home");
    }
}