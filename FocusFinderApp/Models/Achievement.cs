using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Achievement {

    [Key]
    public int id { get; set; }
    [ForeignKey("User")]
    public int? userId { get; set; }
    public int? reviewsLeft { get; set; }
    public int? visits { get; set; }
    public int? citiesVisited { get; set; }
    public int? bookmarks { get; set; }
    }
}
