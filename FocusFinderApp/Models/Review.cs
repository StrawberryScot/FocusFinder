using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Review {

    [Key]
    public int Id { get; set; }
    [ForeignKey("Space")]
    public int SpaceId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]    
    public int Rating { get; set; }

    public DateTime? DateUpdated { get; set; } = DateTime.UtcNow;

    public virtual Space Space { get; set; }
    }
}

