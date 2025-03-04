using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FocusFinderApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace FocusFinderApp.Controllers
{
    // [Authorize] // Ensure only logged-in users can bookmark
    public class BookmarkController : Controller
    {
        private readonly FocusFinderDbContext _dbContext;

        public BookmarkController(FocusFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ✅ Add a bookmark
        [HttpPost]
        public IActionResult Add(int locationId, string redirectUrl)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("You must be logged in to bookmark locations.");
            }

            var user = _dbContext.Users.Include(u => u.Bookmarks)
                    .FirstOrDefault(u => u.Username == username);
            if (user == null) return NotFound("User not found.");

            if (!_dbContext.Bookmarks.Any(b => b.userId == user.Id && b.locationId == locationId))
            {
                _dbContext.Bookmarks.Add(new Bookmark { userId = user.Id, locationId = locationId });
                _dbContext.SaveChanges();
                // Add bookmark to the achievement counter
                Achievement.UpdateUserAchievements(_dbContext, user.Id, "bookmark");
            }

            // Redirect back to the page where the user came from (using the passed redirectUrl)
            return Redirect(redirectUrl ?? "/Home/Index"); // Default to /Home/Index if redirectUrl is null
        }


        // ✅ Show user's bookmarks on their profile
        [Route("Bookmarks")]
        public IActionResult UserBookmarks()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Session");

            var user = _dbContext.Users.Include(u => u.Bookmarks)
                                        .ThenInclude(b => b.Location)
                                        .FirstOrDefault(u => u.Username == username);

            if (user == null) return NotFound("User not found.");

            return View("~/Views/Users/Bookmark.cshtml", user.Bookmarks);
        }
        [HttpPost]
        public IActionResult Remove(int locationId, string redirectUrl)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("You must be logged in to manage bookmarks.");
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return NotFound("User not found.");

            var bookmark = _dbContext.Bookmarks
                .FirstOrDefault(b => b.userId == user.Id && b.locationId == locationId);

            if (bookmark != null)
            {
                _dbContext.Bookmarks.Remove(bookmark);
                _dbContext.SaveChanges();
            }
            // Get redirectUrl from the form submission
            redirectUrl = Request.Form["redirectUrl"].ToString();

            // Redirect based on the passed redirectUrl
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Redirect(redirectUrl);
            }

            // Default redirect if no redirectUrl is passed
            return RedirectToAction("Index", "Home");
        }       
    }
}
