using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FocusFinderApp.Models;

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
        var locations = _dbContext.Locations.ToList(); // Fetch locations

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
        var location = _dbContext.Locations.FirstOrDefault(l => l.Id == id);
        if (location == null)
        {
            Console.WriteLine("Location not found");
            return RedirectToAction("Index");
        }
        return View("~/Views/Home/Location.cshtml", location);
    }

    public IActionResult LocationByCity(string city)
    {
        if (city == null)
        {
            return RedirectToAction("Index");
        }
        var location = _dbContext.Locations.FirstOrDefault(l => l.City == city);
        if (location == null)
        {
            Console.WriteLine("Location not found");
            return RedirectToAction("Index");
        }
        return View("~/Views/Home/Location.cshtml", location);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
