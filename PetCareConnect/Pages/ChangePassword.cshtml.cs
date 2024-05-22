using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace PetCareConnect.Pages
{
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmNewPassword { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToPage("/SignIn");
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

            if (!ModelState.IsValid || NewPassword != ConfirmNewPassword)
            {
                ErrorMessage = "Passwords do not match or form is invalid.";
                return Page();
            }

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    // Verify current password
                    var verifyCommand = new SqlCommand("SELECT Password FROM Users WHERE UserId = @UserId", connection);
                    verifyCommand.Parameters.AddWithValue("@UserId", loggedInUserId.Value);
                    var storedHash = (string)verifyCommand.ExecuteScalar();

                    if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, storedHash))
                    {
                        ErrorMessage = "Current password is incorrect.";
                        return Page();
                    }

                    // Update password
                    var newHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                    var updateCommand = new SqlCommand("UPDATE Users SET Password = @NewPassword WHERE UserId = @UserId", connection);
                    updateCommand.Parameters.AddWithValue("@NewPassword", newHash);
                    updateCommand.Parameters.AddWithValue("@UserId", loggedInUserId.Value);
                    updateCommand.ExecuteNonQuery();

                    SuccessMessage = "Password successfully changed.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while changing the password.";
                Console.WriteLine(ex.Message); // Log the error
                return Page();
            }

            return Page();
        }
    }
}
