using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FocusFinderApp.Models;
using FocusFinderApp.ActionFilters;

namespace FocusFinderApp.Controllers;


[ServiceFilter(typeof(AuthenticationFilter))]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.IsLoggedIn = HttpContext.Items["IsLoggedIn"] as bool?;
        ViewBag.Username = HttpContext.Items["Username"] as string;
        return RedirectToAction("Index", "Location");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
