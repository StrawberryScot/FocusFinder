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
        public IActionResult Add(int locationId)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("You must be logged in to bookmark locations.");
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return NotFound("User not found.");

            // Check if the bookmark already exists
            if (_dbContext.Bookmarks.Any(b => b.userId == user.Id && b.locationId == locationId))
            {
                TempData["InfoMessage"] = "You have already bookmarked this location.";
                return RedirectToAction("Index", "Home");
            }

            var bookmark = new Bookmark
            {
                userId = user.Id,
                locationId = locationId
            };

            _dbContext.Bookmarks.Add(bookmark);
            _dbContext.SaveChanges();
            
            return RedirectToAction("Index", "Home");
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
