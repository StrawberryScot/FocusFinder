using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FocusFinderApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace FocusFinderApp.Controllers
{
    // [Authorize] // Ensure only logged-in users can view achievements
    public class AchievementController : Controller
    {
        private readonly FocusFinderDbContext _dbContext;

        public AchievementController(FocusFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("/Achievements")]
        [HttpGet]
        public IActionResult Achievements()
        {
            ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.Username = HttpContext.Session.GetString("Username");
            
            
            
        //     var userId = HttpContext.Session.GetInt32("UserId");
        //     if (userId == null)
        // {
        //     return RedirectToAction("Index", "Home");
        // }

        var userAchievements = _dbContext.Achievements
            // Creates single instance of achievement or default one so that HTML appears initially with all achievements greyed out, rather than creating a list which is looped through
            .FirstOrDefault(a => a.userId == currentUserId) ?? new Achievement { userId = currentUserId };
            // .Where(a => a.userId == userId)
            // .ToList();

        if (userAchievements == null)
        {
            Console.WriteLine("Achievement not found");
            return RedirectToAction("Index");
        }
        else
        {
            return View("~/Views/Users/Achievement.cshtml", userAchievements);
        }
    }
    }
}