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

        // if (!ViewBag.IsLoggedIn)
        // {
            // var reviews = _dbContext.Reviews.Include(l => l.Reviews).ToList();
            var reviews = _dbContext.Reviews
                .Where( l => l.userId == currentUserId)
                .ToList();
            // if (reviews != null)
            // {
            //     return View("~/Views/Reviews/AllReviews.cshtml", reviews);
            // }
            // else{
            //     return View("~/Views/Reviews/AllReviews.cshtml"); 
            // }

            if (reviews == null)
            {
                Console.WriteLine("Location not found");
                return RedirectToAction("Index");
            }
            else
            {
                return View("~/Views/Reviews/AllReviews.cshtml", reviews);
            }
            
        // }

        // return View("~/Views/Reviews/AllReviews.cshtml");
    }

 
    // [Route("/Reviews/{id}")]
    // [HttpGet]
    // public IActionResult IndivReview(int id)
    // {
    //     // ...
    // }


    // [Route("/Reviews/{id}")]
    // [HttpPost]
    // public IActionResult AddExtReview(int id)
    // {
    //     // ...
    // }

}