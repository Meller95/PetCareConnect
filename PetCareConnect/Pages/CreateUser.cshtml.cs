// File: Pages/CreateUserModel.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using BCrypt.Net;

namespace PetCareConnect.Pages
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            NewUser = new User();
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(NewUser.Username) && !string.IsNullOrEmpty(NewUser.PasswordHash) &&
                !string.IsNullOrEmpty(NewUser.FullName) && !string.IsNullOrEmpty(NewUser.Email))
            {
                NewUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewUser.PasswordHash);

                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("INSERT INTO Users (Username, Password, Email, FullName, CreatedAt) VALUES (@Username, @Password, @Email, @FullName, @CreatedAt)", connection);
                    command.Parameters.AddWithValue("@Username", NewUser.Username);
                    command.Parameters.AddWithValue("@Password", NewUser.PasswordHash);
                    command.Parameters.AddWithValue("@Email", NewUser.Email);
                    command.Parameters.AddWithValue("@FullName", NewUser.FullName);
                    command.Parameters.AddWithValue("@CreatedAt", NewUser.CreatedAt);

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
    }
}
