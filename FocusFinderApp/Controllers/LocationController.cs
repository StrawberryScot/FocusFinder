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

    }

    [Route("/Locations")]
    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Home/Index.cshtml");
    }

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
        
        ViewBag.Location = location;

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
