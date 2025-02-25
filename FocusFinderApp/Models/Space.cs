using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{

public class Space {

    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? HouseNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? ImageURL { get; set; }
    public DateTime? CreatedAt { get; set; }
    }
}


