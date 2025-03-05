using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Controllers;

public class LocationController : Controller
{
    private readonly ILogger<LocationController> _logger;
    private readonly FocusFinderDbContext _dbContext;

    public LocationController(ILogger<LocationController> logger, FocusFinderDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        Console.WriteLine("LocationController instantiated!");
    }

    [Route("/Locations")]
    public IActionResult Index()
    {
        ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
        ViewBag.Username = HttpContext.Session.GetString("Username");

        var locations = _dbContext.Locations.Include(l => l.Reviews).ToList();

        if (ViewBag.IsLoggedIn)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.BookmarkedLocations = _dbContext.Bookmarks
                .Where(b => b.userId == userId)
                .Select(b => b.locationId)
                .ToList();
        }

        return View("~/Views/Home/Index.cshtml", locations);
    }

    [Route("/Locations/{id}")]
    [HttpGet]
    public IActionResult Location(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("Index");
        }
        var location = _dbContext.Locations
            .FirstOrDefault(l => l.Id == id);


        // var reviews = _dbContext.Reviews
            // .Include(l => l.Reviews)
            // .FirstOrDefault(l => l.id == id);

        var reviews = _dbContext.Reviews
            .Where(b => b.locationId == id)
            .ToList();
        ViewBag.reviews = reviews;

        if (location == null)
        {
            Console.WriteLine("Location not found");
            return RedirectToAction("Index");
        }

        // Calculate average rating
        if (location.Reviews != null && location.Reviews.Any())
        {
            ViewBag.AverageRating = location.Reviews.Average(r => r.overallRating);
            ViewBag.AverageRating = Math.Round(ViewBag.AverageRating, 1);
        }
        else
        {
            ViewBag.AverageRating = "No ratings yet";
        }
        // Check if the user is logged in
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        ViewBag.IsLoggedIn = currentUserId != null;

        if (currentUserId != null)
        {
            // Check if the user has visited the location
            var existingVisit = _dbContext.Visits.FirstOrDefault(v => v.locationId == id && v.userId == currentUserId);
            ViewBag.AlreadyVisited = existingVisit != null ? "Already visited" : null;

            // Fetch the user's bookmarked locations
            var bookmarkedLocations = _dbContext.Bookmarks
                .Where(b => b.userId == currentUserId)
                .Select(b => b.locationId)
                .ToList();

            ViewBag.BookmarkedLocations = bookmarkedLocations;
        }
        else
        {
            ViewBag.BookmarkedLocations = new List<int>(); // If user is not logged in, set an empty list
        }

        return View("~/Views/Home/Location.cshtml", location);
    }


    public IActionResult Search(string searchQuery)
    {
        if (searchQuery == null)
        {
            return RedirectToAction("Index");
        }

        searchQuery = searchQuery.ToLower();

        var location = _dbContext.Locations
            .Where(l => l.LocationName.ToLower().Contains(searchQuery) ||
                        l.BuildingIdentifier.ToLower().Contains(searchQuery) ||   
                        l.StreetAddress.ToLower().Contains(searchQuery) ||
                        l.City.ToLower().Contains(searchQuery) ||
                        l.County.ToLower().Contains(searchQuery) ||
                        l.Postcode.ToLower().Contains(searchQuery) ||
                        l.Reviews.Any(r => 
                            (searchQuery.Contains("wifi") && r.freeWifi == true) ||
                            (searchQuery.Contains("toilet") && r.toilets == true) ||
                            (searchQuery.Contains("baby") && r.babychanging == true) ||
                            (searchQuery.Contains("wheelchair") && r.wheelchairAccessible == true) ||
                            (searchQuery.Contains("heating") && r.heating == true) ||
                            (searchQuery.Contains("ac") && r.airConditioning == true) ||
                            (searchQuery.Contains("neuro") && r.neurodivergentFriendly == true) ||
                            (searchQuery.Contains("gluten") && r.glutenFreeOptions == true) ||
                            (searchQuery.Contains("vegan") && r.veganFriendly == true) ||
                            (searchQuery.Contains("office") && r.officeLike == true) ||
                            (searchQuery.Contains("home") && r.homeLike == true) ||
                            (searchQuery.Contains("queer") && r.queerFriendly == true) ||
                            (searchQuery.Contains("group") && r.groupFriendly == true) ||
                            (searchQuery.Contains("pet") && r.petFriendly == true)
                        ))
            .ToList();
        
        return View("~/Views/Home/Index.cshtml", location);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult AddReview(int LocationId, int Rating)
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        var location = _dbContext.Locations.FirstOrDefault(l => l.Id == LocationId);
        
        // Check if the user is logged in (currentUserId is not null)
        if (currentUserId == null)
        {
            return Unauthorized("You must be logged in to add a visit.");
        }
        
        if (location == null)
        {
            return NotFound();
        }
        var newReview = new Review
        {
            locationId = LocationId,
            overallRating = Rating,
            dateLastUpdated = DateTime.UtcNow,
            userId = currentUserId
        };
        _dbContext.Reviews.Add(newReview);
        _dbContext.SaveChanges();
        Achievement.UpdateUserAchievements(_dbContext, currentUserId.Value, "review");
        return RedirectToAction("Location", new { id = LocationId });
    }


    [HttpPost]
    public IActionResult AddVisit(int LocationId)
    {
        var location = _dbContext.Locations.FirstOrDefault(l => l.Id == LocationId);
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        if (location == null)
        {
            return NotFound();
        }

        // Check if the user is logged in (currentUserId is not null)
        if (currentUserId == null)
        {
            return Unauthorized("You must be logged in to add a visit.");
        }

        // Check if the user has already pressed Visited
        var existingVisit = _dbContext.Visits.FirstOrDefault(l => l.locationId == LocationId && l.userId == currentUserId);

        if (existingVisit != null)
        {
            Console.WriteLine("Visit already exists.");
            ViewBag.AlreadyVisited = "Already visited";
        }
        else
        {
            // Add the Visit
            var newVisit = new Visit
            {
                locationId = LocationId,
                dateVisited = DateTime.UtcNow,
                userId = currentUserId
            };

            _dbContext.Visits.Add(newVisit);
            _dbContext.SaveChanges();

            ViewBag.AlreadyVisited = "Already visited";

            // Add visit to the achievement counter
            Achievement.UpdateUserAchievements(_dbContext, currentUserId.Value, "visit");
        }

        return RedirectToAction("Location", new { id = LocationId });
    }

    [HttpPost]
    public IActionResult RemoveVisit(int LocationId)
    {
        var location = _dbContext.Locations.FirstOrDefault(l => l.Id == LocationId);
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        if (location == null)
        {
            return NotFound();
        }

        // Check if the user has already pressed Visited
        var existingVisit = _dbContext.Visits.FirstOrDefault(l => l.locationId == LocationId && l.userId == currentUserId);

        if (existingVisit == null)
        {
            Console.WriteLine("Not visted yet, so can't remove visit.");
            ViewBag.AlreadyVisited = "Not visited yet";  // << may change later
        }
        else
        {
            // Remove the Visit
            _dbContext.Visits.Remove(existingVisit);
            _dbContext.SaveChanges();

            ViewBag.AlreadyVisited = "Not visited yet";  // << may change later
        }

        return RedirectToAction("Location", new { id = LocationId });
    }


    [Route("/SuggestedLocations")]
    [HttpGet]
    public IActionResult AllSuggestedLocations()
    {
        ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
        ViewBag.Username = HttpContext.Session.GetString("Username");

        var suggestedLocations = _dbContext.SuggestedLocations.ToList();

        if (ViewBag.IsLoggedIn)
        {
            Console.WriteLine("SuggestedLocations page - user IS logged in.");
            // return View("~/Views/Home/AllSuggestedLocations.cshtml", suggestedLocations);
        }
        else{
            Console.WriteLine("SuggestedLocations page - user NOT logged in.");
        }
        // return View("~/Views/Home/Index.cshtml");

        if (suggestedLocations == null)
        {
            Console.WriteLine("   suggestedLocations IS null.");
        }
        else{
            Console.WriteLine("   suggestedLocations is NOT null.");
        }
        return View("~/Views/SuggestedLocations/AllSuggestedLocations.cshtml", suggestedLocations);
    }

    [Route("/NewLocationForm")]
    [HttpGet]
    public IActionResult NewLocationForm()
    {
        return View("~/Views/SuggestedLocations/NewLocation.cshtml");
    }

    
    [Route("/NewLocationForm")]
    [HttpPost]
    public IActionResult GetNewLocation(string SuggestedLocationName = null, string Description = null, string BuildingIdentifier = null, string StreetAddress = null, string City = null, string County = null, string Postcode = null, string ImageURL = null, float Latitude = 0, float Longitude = 0)
    {
        var newSuggestedLocation = new SuggestedLocation
        {
            
            SuggestedLocationName = SuggestedLocationName,
            Description = Description,
            BuildingIdentifier = BuildingIdentifier,
            StreetAddress = StreetAddress,
            City = City,
            County = County,
            Postcode = Postcode,
            ImageURL = ImageURL
        };

        if (Latitude != 0)
        {
            newSuggestedLocation.Latitude = Latitude;
        }
        if (Longitude != 0)
        {
            newSuggestedLocation.Longitude = Longitude;
        }

            _dbContext.SuggestedLocations.Add(newSuggestedLocation);
            _dbContext.SaveChanges();
        
        Console.WriteLine("new location form submitted");
        return RedirectToAction("Index");
    }

}
