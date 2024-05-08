using System;

namespace PetCareConnect.Models
{
    public class User
    {
        public int UserId { get; set; } // Aligning with the database column name
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now; // Set default value for CreatedAt
            LastLogin = DateTime.Now; // Set default value for LastLogin
        }
    }
}
