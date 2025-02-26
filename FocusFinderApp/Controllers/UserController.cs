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
        if (_dbContext.Users != null && _dbContext.Users.Any(u => u.Username == user.Username))
        {
            ModelState.AddModelError("Username", "Username is already taken");
        }
        if (_dbContext.Users != null && _dbContext.Users.Any(u => u.Email == user.Email))
        {
            ModelState.AddModelError("Email", "Email is already taken");
        }

        user.JoinDate = DateTime.UtcNow;


        if (ModelState.IsValid)
        {
            _dbContext.Users?.Add(user);
            _dbContext.SaveChanges();
            return RedirectToAction("Home", "Index");
        }
    }
}