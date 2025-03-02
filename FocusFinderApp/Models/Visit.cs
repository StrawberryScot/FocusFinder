using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Visit {

    [Key]
    public int id { get; set; }

    [ForeignKey("user")]
    public int? userId { get; set; }

    [ForeignKey("Location")]
    public int? locationId { get; set; }

    public DateTime? dateVisited { get; set; } = DateTime.UtcNow;

    public virtual Location? Location { get; set; }
    
    public virtual User? User { get; set; }
    }
}
