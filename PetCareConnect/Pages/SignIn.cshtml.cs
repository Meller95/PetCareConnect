using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using BCrypt.Net;
using System;

namespace PetCareConnect.Pages
{
    public class SignInModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (IsLoginValid(Username, Password))
            {
                HttpContext.Session.SetString("Username", Username);

                // Get the UserId from the username
                int userId = GetUserIdByUsername(Username);
                HttpContext.Session.SetInt32("UserId", userId);

                // Update the LastLogin field in the database
                UpdateLastLogin(userId);

                return RedirectToPage("/YourProfile");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }

        private bool IsLoginValid(string username, string password)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT Password FROM Users WHERE LOWER(Username) = LOWER(@Username)", connection);
                command.Parameters.AddWithValue("@Username", username);

                var dbPassword = command.ExecuteScalar() as string;

                if (dbPassword != null && BCrypt.Net.BCrypt.Verify(password, dbPassword))
                {
                    return true;
                }
                return false;
            }
        }

        private int GetUserIdByUsername(string username)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT UserId FROM Users WHERE LOWER(Username) = LOWER(@Username)", connection);
                command.Parameters.AddWithValue("@Username", username);

                return (int)command.ExecuteScalar();
            }
        }

        private void UpdateLastLogin(int userId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("UPDATE Users SET LastLogin = @LastLogin WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }
    }
}
