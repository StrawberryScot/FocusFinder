using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FocusFinderApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Controllers;

public class UsersController : Controller
{
    private readonly FocusFinderDbContext _dbContext;
    private readonly ILogger<UsersController> _logger;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UsersController(FocusFinderDbContext dbContext, ILogger<UsersController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Route("/register")]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [Route("/register")]
    [HttpPost]
    public IActionResult Register(User user)
    {

        // 🔹 Validate required fields
        var requiredFields = new Dictionary<string, string>
        {
            { "Password", user.Password ?? string.Empty },
            { "Username", user.Username ?? string.Empty },
            { "Email", user.Email ?? string.Empty }
        };

        foreach (var field in requiredFields)
        {
            if (string.IsNullOrWhiteSpace(field.Value))
            {
                ModelState.AddModelError(field.Key, $"{field.Key} is required");
            }
        }

        if (ModelState.IsValid && _dbContext.Users != null)
        {
            if (_dbContext.Users.Any(u => u.Username == user.Username))
                ModelState.AddModelError("Username", "Username is already taken");
            
            if (_dbContext.Users.Any(u => u.Email == user.Email))
                ModelState.AddModelError("Email", "Email is already taken");
        }
            

        if (!ModelState.IsValid)
        {
            return View(user);
        }

        if (user.Password != null)
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
        }

        user.JoinDate = DateTime.UtcNow;

        user.ProfilePicture ??= "/images/default-picture/default.png";
        
        
        _dbContext.Users?.Add(user);
        _dbContext.SaveChanges();
        return RedirectToAction("Index", "Home");
        
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("/Profile/{username}")]
    [HttpGet]
    public IActionResult Profile(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required");
        }

        var user = _dbContext.Users?.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

        if (user == null)
        {
            return NotFound("User not found");
        }

        return View(user);
    }
}