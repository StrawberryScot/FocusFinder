using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Review {

    [Key]
    public int id { get; set; }
    [ForeignKey("Location")]
    public int? locationId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]    
    public int? overallRating { get; set; }

    public DateTime? dateLastUpdated { get; set; } = DateTime.UtcNow;

    public virtual Location? Location { get; set; }
    }
}
