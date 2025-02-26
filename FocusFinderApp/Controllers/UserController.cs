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

        if (string.IsNullOrWhiteSpace(user.Password))
        {
            ModelState.AddModelError("Password", "Password is required");
        }
        if (string.IsNullOrWhiteSpace(user.Username))
        {
            ModelState.AddModelError("Username", "Username is required");
        }
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            ModelState.AddModelError("Email", "Email is required");
        }
        if (_dbContext.Users != null && _dbContext.Users.Any(u => u.Username == user.Username))
        {
            ModelState.AddModelError("Username", "Username is already taken");
        }
        if (_dbContext.Users != null && _dbContext.Users.Any(u => u.Email == user.Email))
        {
            ModelState.AddModelError("Email", "Email is already taken");
        }


        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var passwordHasher = new PasswordHasher<User>();
        if (user.Password != null)
        {
            user.Password = passwordHasher.HashPassword(user, user.Password);
        }

        user.JoinDate = DateTime.UtcNow;

        if (string.IsNullOrEmpty(user.ProfilePicture))
        {
            user.ProfilePicture = "/images/default-picture/default.png";
        }
        
        
        _dbContext.Users?.Add(user);
        _dbContext.SaveChanges();
        return RedirectToAction("Index", "Home");
        
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}