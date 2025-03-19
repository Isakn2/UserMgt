using Microsoft.AspNetCore.Identity;

namespace UserManagementApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; } // Required
        public DateTime LastLoginTime { get; set; }
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";
    }
}