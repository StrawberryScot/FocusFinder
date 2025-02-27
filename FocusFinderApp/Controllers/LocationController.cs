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
    [HttpGet]
    public IActionResult Index()
    {
        var locations = _dbContext.Locations
            .Include(l => l.Reviews) // Include Reviews to fetch them with Locations
            .ToList(); 

        if (locations == null || !locations.Any())
        {
            _logger.LogWarning("No locations found in the database.");
        }
        else
        {
            _logger.LogInformation($"Retrieved {locations.Count} locations from the database.");
        }

        return View("~/Views/Home/Index.cshtml", locations);
    }

    // [Route("/Locations")]
    // [HttpGet]
    // public IActionResult Index()
    // {
    //     var locations = _dbContext.Locations.ToList() ?? new List<Location>(); // Get all locations from the database (last bit ensures its never null)
    //     return View("~/Views/Home/Index.cshtml", locations); // Adding locations at the end sends the locations list to the view
    // }

    [Route("/Locations/{id}")]
    [HttpGet]
    public IActionResult Location(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("Index");
        }
        var location = _dbContext.Locations
            .Include(l => l.Reviews)
            .FirstOrDefault(l => l.Id == id);

        if (location == null)
        {
            Console.WriteLine("Location not found");
            return RedirectToAction("Index");
        }
        
        // ViewBag.Location = location; << no longer needed as using @model in cshtml

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

        return View("~/Views/Home/Location.cshtml", location);
    }

    public IActionResult LocationByCity(string city)
    {
        if (city == null)
        {
            return RedirectToAction("Index");
        }
        var location = _dbContext.Locations
            .Where( l => l.City.ToLower() == city.ToLower())
            .ToList();
        
        if (location == null)
        {
            Console.WriteLine("Location not found");
            return RedirectToAction("Index");
        }
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
        var location = _dbContext.Locations.FirstOrDefault(l => l.Id == LocationId);
        if (location == null)
        {
            return NotFound();
        }
        var newReview = new Review
        {
            locationId = LocationId,
            overallRating = Rating,
            dateLastUpdated = DateTime.UtcNow
        };
        _dbContext.Reviews.Add(newReview);
        _dbContext.SaveChanges();
        return RedirectToAction("Location", new { id = LocationId });
    }
}
