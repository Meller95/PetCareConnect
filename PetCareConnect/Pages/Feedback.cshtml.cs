using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System;

namespace PetCareConnect.Pages
{
    public class FeedbackModel : PageModel
    {
        [BindProperty]
        public Feedback Feedback { get; set; }

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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Feedback.UserId = loggedInUserId.Value;
            Feedback.FeedbackDate = DateTime.Now;

            using (var connection = DB_Connection.GetConnection())
            {
                string query = "INSERT INTO Feedback (UserId, FeedbackDate, FeedbackText, Rating) VALUES (@UserId, @FeedbackDate, @FeedbackText, @Rating)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", Feedback.UserId);
                    command.Parameters.AddWithValue("@FeedbackDate", Feedback.FeedbackDate);
                    command.Parameters.AddWithValue("@FeedbackText", Feedback.FeedbackText);
                    command.Parameters.AddWithValue("@Rating", Feedback.Rating);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Index");
        }
    }
}
