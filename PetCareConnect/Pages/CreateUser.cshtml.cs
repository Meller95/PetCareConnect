using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using PetCareConnect.Models;
using BCrypt.Net;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetCareConnect.Pages
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; }

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            NewUser = new User();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(NewUser.Username) && !string.IsNullOrEmpty(NewUser.PasswordHash) &&
                !string.IsNullOrEmpty(NewUser.FullName) && !string.IsNullOrEmpty(NewUser.Email))
            {
                // Check if username already exists (case-insensitive)
                if (IsUsernameTaken(NewUser.Username))
                {
                    Message = "Username already exists.";
                    return Page();
                }

                // Password validation
                if (!IsPasswordValid(NewUser.PasswordHash))
                {
                    Message = "Password must be at least 8 characters long and contain both uppercase and lowercase letters.";
                    return Page();
                }

                NewUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewUser.PasswordHash);

                string profilePictureUrl = null;
                if (ProfilePicture != null && ProfilePicture.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images/Users", $"{NewUser.Username}.jpg");

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(stream);
                    }

                    profilePictureUrl = $"/images/Users/{NewUser.Username}.jpg";
                }

                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("INSERT INTO Users (Username, Password, Email, FullName, CreatedAt, ProfilePictureUrl) VALUES (@Username, @Password, @Email, @FullName, @CreatedAt, @ProfilePictureUrl)", connection);
                    command.Parameters.AddWithValue("@Username", NewUser.Username);
                    command.Parameters.AddWithValue("@Password", NewUser.PasswordHash);
                    command.Parameters.AddWithValue("@Email", NewUser.Email);
                    command.Parameters.AddWithValue("@FullName", NewUser.FullName);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@ProfilePictureUrl", (object)profilePictureUrl ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }

                Message = "User created successfully!";
                return RedirectToPage("/SignIn");
            }
            else
            {
                Message = "All fields are required.";
                return Page();
            }
        }

        private bool IsUsernameTaken(string username)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE LOWER(Username) = LOWER(@Username)", connection);
                command.Parameters.AddWithValue("@Username", username);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);

            return hasUpperCase && hasLowerCase;
        }
    }
}
