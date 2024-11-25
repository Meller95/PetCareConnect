using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Collections.Generic;

namespace PetCareConnect.Pages
{
    public class EditAssignmentModel : PageModel
    {
        [BindProperty]
        public Assignment Assignment { get; set; }

        public List<Pets> RegisteredPets { get; set; }
        public List<SelectListItem> TaskTypes { get; set; } // Dropdown list for task types

        public IActionResult OnGet(int assignmentId)
        {
            // Get the current user ID
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (LoggedInUserId == null)
            {
                return RedirectToPage("/Login");
            }

            // Load the assignment details
            Assignment = GetAssignmentDetails(assignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Load user's registered pets from the database
            RegisteredPets = GetPetsForUser((int)LoggedInUserId);

            // Populate dropdown list for task types
            TaskTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Walk", Text = "Take the pet for a walk" },
                new SelectListItem { Value = "Visit", Text = "A small visit with the pet" },
                new SelectListItem { Value = "OvernightOwner", Text = "Overnight pet sitting at the owner's home" },
                new SelectListItem { Value = "OvernightSitter", Text = "Overnight pet sitting at the pet sitter's home" }
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (LoggedInUserId == null)
            {
                return RedirectToPage("/Login");
            }

            Assignment.UserId = (int)LoggedInUserId; // Ensure UserId is set

            // Update the assignment in the database
            try
            {
                using (SqlConnection connection = DB_Connection.GetConnection())
                {
                    if (connection != null)
                    {
                        string query = "UPDATE Assignments SET PetId = @PetId, StartDate = @StartDate, EndDate = @EndDate, TaskType = @TaskType, FeedingSchedule = @FeedingSchedule, FoodAmount = @FoodAmount, Title = @Title, City = @City, Comments = @Comments, Payment = @Payment WHERE AssignmentId = @AssignmentId";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@PetId", Assignment.PetId);
                            command.Parameters.AddWithValue("@StartDate", Assignment.StartDate);
                            command.Parameters.AddWithValue("@EndDate", Assignment.EndDate);
                            command.Parameters.AddWithValue("@TaskType", Assignment.TaskType);
                            command.Parameters.AddWithValue("@FeedingSchedule", Assignment.FeedingSchedule);
                            command.Parameters.AddWithValue("@FoodAmount", Assignment.FoodAmount);
                            command.Parameters.AddWithValue("@Title", Assignment.Title);
                            command.Parameters.AddWithValue("@City", Assignment.City);
                            command.Parameters.AddWithValue("@Comments", Assignment.Comments);
                            command.Parameters.AddWithValue("@Payment", Assignment.Payment);
                            command.Parameters.AddWithValue("@AssignmentId", Assignment.AssignmentId);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Assignment updated successfully.");
                                return RedirectToPage("/YourProfile");
                            }
                            else
                            {
                                Console.WriteLine("No rows affected.");
                                return Page();  // Return to page with error message
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to connect to the database.");
                        return Page();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
                return Page();
            }
        }

        private Assignment GetAssignmentDetails(int assignmentId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT AssignmentId, UserId, PetId, StartDate, EndDate, TaskType, FeedingSchedule, FoodAmount, Title, City, Comments, Payment FROM Assignments WHERE AssignmentId = @AssignmentId", connection);
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
                            Payment = reader.GetDecimal(11)
                        };
                    }
                }
            }
            return null;
        }

        public List<Pets> GetPetsForUser(int userId)
        {
            var pets = new List<Pets>();
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT PetId, Name, Species, Breed, Age, Info FROM Pets WHERE OwnerId = @OwnerId", connection);
                command.Parameters.AddWithValue("@OwnerId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pets.Add(new Pets
                        {
                            PetId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Species = reader.GetString(2),
                            Breed = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Age = reader.GetInt32(4),
                            Info = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }
                }
            }
            return pets;
        }
    }
}
