using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

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
        if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
        {
            var (isValid, userId) = IsValidUser(Username, Password);
            if (isValid)
            {
                HttpContext.Session.SetInt32("UserId", userId.Value); // Store user ID in session
                HttpContext.Session.SetString("Username", Username); // Optionally store username as well
                return RedirectToPage("/YourProfile");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }

        ErrorMessage = "Username and password are required.";
        return Page();
    }




    private (bool, int?) IsValidUser(string username, string password)
    {
        using (var connection = DB_Connection.GetConnection())
        {
            // Prepare the SQL command to retrieve the user ID and hashed password
            var command = new SqlCommand("SELECT UserId, Password FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int userId = reader.GetInt32(0);
                    string storedHash = reader.GetString(1);

                    if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        return (true, userId);
                    }
                }
            }
        }
        return (false, null);
    }


}
