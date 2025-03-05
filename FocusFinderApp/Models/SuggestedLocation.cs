using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class SuggestedLocation {

    [Key]
    public int Id { get; set; }

    public string? SuggestedLocationName { get; set; }

    public string? Description { get; set; }

    public string? BuildingIdentifier { get; set; }

    public string? StreetAddress { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    [RegularExpression(@"^[A-Za-z0-9\s\-]+$", ErrorMessage = "Invalid postcode format")]
    public string? Postcode { get; set; }

    public string? ImageURL { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public string? locationURL { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
    

    // public ICollection<Review>? Reviews { get; set; }
    }
}


