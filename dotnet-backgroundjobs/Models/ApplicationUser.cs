using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnet_backgroundjobs.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string City { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
