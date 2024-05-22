using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace PetCareConnect.Pages
{
    public class ChangeEmailModel : PageModel
    {
        [BindProperty]
        public string CurrentEmail { get; set; }

        [BindProperty]
        public string NewEmail { get; set; }

        [BindProperty]
        public string ConfirmNewEmail { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToPage("/SignIn");
            }

            // Retrieve the current email address from the database
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("SELECT Email FROM Users WHERE UserId = @UserId", connection);
                    command.Parameters.AddWithValue("@UserId", loggedInUserId.Value);
                    CurrentEmail = (string)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while retrieving the current email.";
                Console.WriteLine(ex.Message); // Log the error
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToPage("/SignIn");
            }

            if (!ModelState.IsValid || NewEmail != ConfirmNewEmail)
            {
                ErrorMessage = "Emails do not match or form is invalid.";
                return Page();
            }

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    // Update email
                    var updateCommand = new SqlCommand("UPDATE Users SET Email = @NewEmail WHERE UserId = @UserId", connection);
                    updateCommand.Parameters.AddWithValue("@NewEmail", NewEmail);
                    updateCommand.Parameters.AddWithValue("@UserId", loggedInUserId.Value);
                    updateCommand.ExecuteNonQuery();

                    SuccessMessage = "Email successfully changed.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while changing the email.";
                Console.WriteLine(ex.Message); // Log the error
                return Page();
            }

            return Page();
        }
    }
}
