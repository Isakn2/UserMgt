using Microsoft.AspNetCore.Identity;

namespace UserManagementApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; } // Required
        public required string Email { get; set; } // Required
        public required string PasswordHash { get; set; } // Required
        public DateTime LastLoginTime { get; set; }
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";
    }
}