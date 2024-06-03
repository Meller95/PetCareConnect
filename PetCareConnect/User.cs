using System;

namespace PetCareConnect.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string ProfilePictureUrl { get; set; } // Add this property

        public User()
        {
            CreatedAt = DateTime.Now;
            LastLogin = DateTime.Now;
        }
    }
}
