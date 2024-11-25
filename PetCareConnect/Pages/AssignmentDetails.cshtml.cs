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
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
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
                var command = new SqlCommand(@"
                    SELECT a.AssignmentId, a.UserId, a.PetId, a.StartDate, a.EndDate, a.TaskType,
                           a.FeedingSchedule, a.FoodAmount, a.Title, a.City, a.Comments, a.Payment,
                           p.PictureUrl, p.Name AS PetName, p.Species, u.Username AS UserName
                    FROM Assignments a
                    JOIN Pets p ON a.PetId = p.PetId
                    JOIN Users u ON a.UserId = u.UserId
                    WHERE a.AssignmentId = @AssignmentId", connection);
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
                            FoodAmount = reader.GetString(7),
                            Title = reader.IsDBNull(8) ? null : reader.GetString(8),
                            City = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Comments = reader.IsDBNull(10) ? null : reader.GetString(10),
                            Payment = reader.GetDecimal(11),
                            PictureUrl = reader.IsDBNull(12) ? null : reader.GetString(12),
                            PetName = reader.GetString(13),
                            Species = reader.GetString(14),
                            UserName = reader.GetString(15)
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
