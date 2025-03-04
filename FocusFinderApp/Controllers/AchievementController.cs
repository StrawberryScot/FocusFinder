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
            .Where(a => a.userId == userId)
            .ToList(); // Ensure it's a List

        return View("~/Views/Users/Achievement.cshtml", userAchievements);
    }
}
}


//         // List all achievements on the user’s achievements page
//         [Route("Achievements")]
//         [HttpGet]
//         public IActionResult List()
//         {
//             int? currentUserId = HttpContext.Session.GetInt32("UserId");

//             if (currentUserId == null)
//             {
//                 return Unauthorized("You must be logged in to view achievements.");
//             }

//             var achievements = _dbContext.Achievements
//                                         .Where(a => a.userId == currentUserId.Value)
//                                         .FirstOrDefault();

//             if (achievements == null)
//             {
//                 return NotFound("No achievements found for this user.");
//             }

//             return View(achievements); // Pass achievements to the view
//         }
//     }
// }
