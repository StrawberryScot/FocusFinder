using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Review {

// GENERAL 1-5* REVIEW
    [Key]
    public int id { get; set; }
    [ForeignKey("Location")]
    public int? locationId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]    
    public int? overallRating { get; set; }

    public DateTime? dateLastUpdated { get; set; } = DateTime.UtcNow;
    public int? userId { get; set; } // TBC on '?'

// TAGS
    public bool? freeWifi  { get; set; }
    public bool? toilets  { get; set; }
    public bool? babychanging  { get; set; }
    public bool? wheelchairAccessible  { get; set; }
    public bool? heating { get; set; }
    public bool? airConditioning  { get; set; }
    public bool? neurodivergentFriendly  { get; set; }
    public bool? glutenFreeOptions  { get; set; }
    public bool? veganFriendly  { get; set; }
    public bool? officeLike  { get; set; }
    public bool? homeLike  { get; set; }
    public bool? queerFriendly  { get; set; }
    public bool? groupFriendly  { get; set; }
    public bool? petFriendly  { get; set; }

// 1-5 EXT REVIEWS
    public int? cleanliness  { get; set; }
    public int? noiseLevel  { get; set; }
    public int? seatingAvailability  { get; set; }
    public int? wifiSpeed  { get; set; }
    public int? chargingPointAvailability  { get; set; }

// COMMENTS
    public string? comments { get; set; }


    public virtual Location? Location { get; set; }
    }
}
