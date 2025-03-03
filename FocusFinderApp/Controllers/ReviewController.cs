using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FocusFinderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusFinderApp.Controllers;

public class ReviewController : Controller
{
    private readonly ILogger<ReviewController> _logger;
    private readonly FocusFinderDbContext _dbContext;

    public ReviewController(ILogger<ReviewController> logger, FocusFinderDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        Console.WriteLine("ReviewController instantiated!");
    }


    [Route("/Reviews")]
    [HttpGet]
    public IActionResult AllReviews()
    {
        ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        ViewBag.Username = HttpContext.Session.GetString("Username");

        var reviews = _dbContext.Reviews
            .Where( l => l.userId == currentUserId)
            .ToList();

        if (reviews == null)
        {
            Console.WriteLine("Location not found");
            return RedirectToAction("Index");
        }
        else
        {
            return View("~/Views/Reviews/AllReviews.cshtml", reviews);
        }

    }

 
    [Route("/Reviews/{id}")]
    [HttpGet]
    public IActionResult IndivReview(int reviewId)
    {

        ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        ViewBag.Username = HttpContext.Session.GetString("Username");

        var review = _dbContext.Reviews
            .FirstOrDefault(l => l.id == reviewId); // << very important ! 

        if (review == null)
        {
            Console.WriteLine("Review not found");
            return View("~/Views/Reviews/IndivReview.cshtml");
        }

        return View("~/Views/Reviews/IndivReview.cshtml", review);

    }


    // [Route("/Reviews/{id}")]
    // [HttpPost]
    // public IActionResult AddExtReview(int id)
    // {
    //     // ...
    // }

}