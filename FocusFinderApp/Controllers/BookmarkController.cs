using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FocusFinderApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace FocusFinderApp.Controllers
{
    // [Authorize] // Ensure only logged-in users can bookmark
    // [Route("Bookmark")]
    public class BookmarkController : Controller
    {
        private readonly FocusFinderDbContext _dbContext;

        public BookmarkController(FocusFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Add(int locationId, string redirectUrl)
        {
            // Retrieve username from the session
            var username = HttpContext.Session.GetString("Username");

            // Ensure the user is logged in
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("You must be logged in to bookmark locations.");
            }

            // Retrieve the user along with their Bookmarks and Location
            var user = _dbContext.Users
                .Include(u => u.Bookmarks)  // Include Bookmarks to check for existing bookmarks
                .ThenInclude(b => b.Location)  // Include Location for each bookmark
                .FirstOrDefault(u => u.Username == username);

            // If user is not found, return an error
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the bookmark already exists
            if (!_dbContext.Bookmarks.Any(b => b.userId == user.Id && b.locationId == locationId))
            {
                // Add the new bookmark and save changes to the database
                _dbContext.Bookmarks.Add(new Bookmark { userId = user.Id, locationId = locationId });
                _dbContext.SaveChanges();

                // Optionally, update achievements or other logic (if applicable)
                Achievement.UpdateUserAchievements(_dbContext, user.Id, "bookmark");

                Console.WriteLine($"Bookmark added: User {user.Id} -> Location {locationId}");
            }
            else
            {
                Console.WriteLine($"Bookmark already exists: User {user.Id} -> Location {locationId}");
            }

            // Redirect to the provided URL or fallback to home page
            return Redirect(redirectUrl ?? "/Home/Index");
        }


        // Show user's bookmarks on their profile
        [Route("/Bookmarks")]
        public IActionResult UserBookmarks()
        {
        
            ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.Username = HttpContext.Session.GetString("Username");

            var bookmarks = _dbContext.Bookmarks
                .Where( l => l.userId == currentUserId)
                .Include(b => b.Location) 
                .ToList();

            if (bookmarks == null)
            {
                Console.WriteLine("Bookmark not found");
                return RedirectToAction("Index");
            }
            else
            {
                return View("~/Views/Users/Bookmark.cshtml", bookmarks);
                // return View("~/Views/Users/Bookmark.cshtml", user.Bookmarks);
            }
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
