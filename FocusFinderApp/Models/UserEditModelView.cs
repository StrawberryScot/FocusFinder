using System.ComponentModel.DataAnnotations;

namespace FocusFinderApp.Models
{
    public class UserEditModelView
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? DefaultCity { get; set; }

        public string? ProfilePicture { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }
    }
}