using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Review {

    [Key]
    public int Id { get; set; }
    [ForeignKey("Location")]
    public int LocationId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]    
    public int Rating { get; set; }

    public DateTime? DateUpdated { get; set; } = DateTime.UtcNow;

    public virtual Location Location { get; set; }
    }
}

