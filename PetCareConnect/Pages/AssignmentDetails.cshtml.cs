using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System;

namespace PetCareConnect.Pages
{
    public class AssignmentDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int AssignmentId { get; set; }

        public Assignment Assignment { get; set; }
        public Pets Pet { get; set; }

        public IActionResult OnGet()
        {
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (LoggedInUserId == null)
            {
                return RedirectToPage("/SignIn");
            }
            // Fetch assignment details
            Assignment = GetAssignmentDetails(AssignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Fetch pet details
            Pet = GetPetDetails(Assignment.PetId);

            return Page();
        }

        public IActionResult OnPostChangeAssignment()
        {
            // Redirect to the page where you can change the assignment
            return RedirectToPage("/EditAssignment", new { assignmentId = AssignmentId });
        }

        public IActionResult OnPostDeleteAssignment()
        {
            // Delete the assignment
            DeleteAssignment(AssignmentId);
            return RedirectToPage("/YourProfile");
        }

        private Assignment GetAssignmentDetails(int assignmentId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT * FROM Assignments WHERE AssignmentId = @AssignmentId", connection);
                command.Parameters.AddWithValue("@AssignmentId", assignmentId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Assignment
                        {
                            AssignmentId = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            PetId = reader.GetInt32(2),
                            StartDate = reader.GetDateTime(3),
                            EndDate = reader.GetDateTime(4),
                            TaskType = reader.GetString(5),
                            FeedingSchedule = reader.GetString(6),
                            FoodAmount = reader.GetString(7)
                        };
                    }
                }
            }
            return null;
        }

        private Pets GetPetDetails(int petId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT * FROM Pets WHERE PetId = @PetId", connection);
                command.Parameters.AddWithValue("@PetId", petId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Pets
                        {
                            PetId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Species = reader.GetString(2),
                            Breed = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Age = reader.GetInt32(4),
                            Info = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                    }
                }
            }
            return null;
        }

        private void DeleteAssignment(int assignmentId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("DELETE FROM Assignments WHERE AssignmentId = @AssignmentId", connection);
                command.Parameters.AddWithValue("@AssignmentId", assignmentId);
                command.ExecuteNonQuery();
            }
        }
    }
}

