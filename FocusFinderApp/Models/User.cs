using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFinderApp.Models
{



public class User {

    [Key]
    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "First name must be less than 50 characters")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "Last name must be less than 50 characters")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50)]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string? Password { get; set; }

    [NotMapped]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please confirm your password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

    [RegularExpression(@"(https?:\/\/.*\.(?:png|jpg|jpeg|gif|svg))", ErrorMessage = "Please enter a valid image URL (png, jpg, jpeg, gif, svg).")]
    public string? ProfilePicture { get; set; }

    private DateTime _joinDate;
    public DateTime JoinDate
    {
        get => _joinDate;
        set => _joinDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public string? DefaultCity { get; set; }

    [NotMapped]
    public string? FullName => 
    string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
    ? null
    : $"{FirstName} {LastName}".Trim();

    public virtual List<Bookmark>? Bookmarks { get; set; } = new List<Bookmark>();

    }
}



