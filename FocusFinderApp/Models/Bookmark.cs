using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Bookmark {

    [Key]
    public int id { get; set; }
    [ForeignKey("User")]
    public int? userId { get; set; }
    [ForeignKey("Location")]
    public int? locationId { get; set; }
    public virtual User? User { get; set; }
    public virtual Location? Location { get; set; }
    }
}
