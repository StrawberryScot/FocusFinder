using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FocusFinderApp.Models
{



public class User {

    [Key]
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    }
}



