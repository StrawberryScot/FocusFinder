using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Space {

    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public address? Address { get; set; }
    public string? ImageURL { get; set; }
    public DateTime? CreatedAt { get; set; }
    }
}


