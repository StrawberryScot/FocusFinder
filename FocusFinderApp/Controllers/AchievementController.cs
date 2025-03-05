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

        [Route("Achievements")]
        [HttpGet]
        public IActionResult Achievements()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var userAchievements = _dbContext.Achievements
            // Creates single instance of achievement or default one so that HTML appears initially with all achievements greyed out, rather than creating a list which is looped through
            .FirstOrDefault(a => a.userId == userId) ?? new Achievement { userId = userId };
            // .Where(a => a.userId == userId)
            // .ToList();

        return View("~/Views/Users/Achievement.cshtml", userAchievements);
    }
}
}

