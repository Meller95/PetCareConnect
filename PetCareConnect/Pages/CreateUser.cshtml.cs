using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace PetCareConnect.Pages
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                // Hashing the password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

                // Insert username and hashed password into the database
                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("INSERT INTO Users (Username, Password) VALUES (@Username, @Password)", connection);
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    command.ExecuteNonQuery();
                }

                Message = "User created successfully!";
            }
            else
            {
                Message = "Please provide both username and password.";
            }
        }
    }
}
