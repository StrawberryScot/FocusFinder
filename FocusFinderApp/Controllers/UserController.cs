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


    [Route("/Profile/Edit")]
    [HttpGet]
    public IActionResult Edit(string username)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (string.IsNullOrWhiteSpace(loggedInUser))
        {
            TempData["ErrorMessage"] = "You must be logged in to edit your profile";
            return RedirectToAction("Login", "Session");
        }

        var userToEdit = _dbContext.Users?.FirstOrDefault(u => u.Username.ToLower() == loggedInUser.ToLower());

        if (userToEdit == null)
        {
            return NotFound("User not found");
        }

        return View(userToEdit);
    }


    [Route("/Profile/Edit")]
    [HttpPost]
    public IActionResult Edit(UserEditModel model)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (string.IsNullOrWhiteSpace(loggedInUser))
        {
            TempData["ErrorMessage"] = "You must be logged in to edit your profile";
            return RedirectToAction("Login", "Session");
        }

        var userToEdit = _dbContext.Users?.FirstOrDefault(u => u.Username.ToLower() == loggedInUser.ToLower());

        if (userToEdit == null)
        {
            return NotFound("User not found");
        }
        
        if (!string.IsNullOrWhiteSpace(model.FirstName))
        {
            userToEdit.FirstName = model.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(model.LastName))
        {
            userToEdit.LastName = model.LastName;
        }

        if (!string.IsNullOrWhiteSpace(model.DefaultCity))
        {
            userToEdit.DefaultCity = model.DefaultCity;
        }

        

        if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
            var extension = Path.GetExtension(model.ProfilePictureFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ProfilePicture", "Only JPG, JPEG, PNG, and SVG files are allowed.");
                return View(userToEdit);
            }

            try 
            {
                var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile-pictures", loggedInUser);
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(userFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfilePictureFile.CopyTo(fileStream);
                }

                userToEdit.ProfilePicture = $"/images/profile-pictures/{loggedInUser}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ProfilePicture", $"An error occurred while uploading the image: {ex.Message}");
                return View(userToEdit);
            }
        }
        

        _dbContext.Users?.Update(userToEdit);
        _dbContext.SaveChanges();

        TempData["SuccessMessage"] = "Profile updated successfully";
        return RedirectToAction("Profile", "Users", new { username = userToEdit.Username });
    }

    [Route("/Profile/ChangePassword")]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (string.IsNullOrWhiteSpace(loggedInUser))
        {
            TempData["ErrorMessage"] = "You must be logged in to change your password";
            return RedirectToAction("Login", "Session");
        }

        return View(new ChangePasswordModelView { Username = loggedInUser });
    }

    [Route("/Profile/ChangePassword")]
    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordModelView model)
    {
        var loggedInUser = HttpContext.Session.GetString("Username");
        if (string.IsNullOrWhiteSpace(loggedInUser))
        {
            TempData["ErrorMessage"] = "You must be logged in to change your password";
            return RedirectToAction("Login", "Session");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userToEdit = _dbContext.Users?.FirstOrDefault(u => u.Username.ToLower() == loggedInUser.ToLower());

        if (userToEdit == null)
        {
            return NotFound("User not found");
        }

        var result = _passwordHasher.VerifyHashedPassword(userToEdit, userToEdit.Password, model.CurrentPassword);

        if (result != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("CurrentPassword", "Incorrect current password");
            return View(model);
        }

        userToEdit.Password = _passwordHasher.HashPassword(userToEdit, model.NewPassword);
        _dbContext.Users?.Update(userToEdit);
        _dbContext.SaveChanges();

        TempData["SuccessMessage"] = "Password updated successfully";
        return RedirectToAction("Profile", "Users", new { username = userToEdit.Username });
    }
}

  