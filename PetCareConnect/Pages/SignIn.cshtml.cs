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
            if (IsValidUser(Username, Password))
            {
                // Assuming successful sign-in, redirect to a different page
                return RedirectToPage("/Index");
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

    private bool IsValidUser(string username, string password)
    {
        // Establish a connection to the database
        using (var connection = DB_Connection.GetConnection())
        {
            // Prepare the SQL command to retrieve the hashed password
            var command = new SqlCommand("SELECT Password FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            // Execute the command and retrieve the hashed password
            var result = command.ExecuteScalar();
            if (result != null)
            {
                string storedHash = result.ToString();
                Console.WriteLine("Stored Hash: " + storedHash);  // Check the format here
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
            else
            {
                Console.WriteLine("No matching user found or hash retrieval failed.");
            }

        }
        return false;
    }

}
