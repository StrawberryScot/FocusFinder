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

            TempData["SuccessMessage"] = "Location bookmarked successfully!";
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
        // [Authorize] // Ensure only logged-in users can remove bookmarks
        public IActionResult Remove(int bookmarkId)
        {
            var bookmark = _dbContext.Bookmarks.Find(bookmarkId);
    
            if (bookmark != null)
            {
                _dbContext.Bookmarks.Remove(bookmark);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("UserBookmarks");
        }
    }
}
