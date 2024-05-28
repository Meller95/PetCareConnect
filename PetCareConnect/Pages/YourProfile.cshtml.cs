using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Collections.Generic;

namespace PetCareConnect.Pages
{
    public class YourProfileModel : PageModel
    {
        public User User { get; set; }
        public List<Pets> Pets { get; set; }
        public List<Assignment> Assignments { get; set; }
        public List<Assignment> BookedAssignments { get; set; } // New

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                User = GetUserDetails(username);
                if (User != null)
                {
                    Pets = GetPetsForUser(User.UserId);
                    Assignments = GetAssignmentsForUser(User.UserId);
                    BookedAssignments = GetBookedAssignmentsForUser(User.UserId); // New
                }
                else
                {
                    return RedirectToPage("/SignIn");
                }
            }
            else
            {
                return RedirectToPage("/SignIn");
            }
            return Page();
        }

        public IActionResult OnPostDeletePet(int petId)  // Ensure parameter name 'petId' is correctly cased
        {
            if (petId <= 0)
            {
                return BadRequest("Invalid ID provided.");
            }

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("DELETE FROM Pets WHERE PetId = @PetId", connection);
                    command.Parameters.AddWithValue("@PetId", petId);
                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        return NotFound("Pet not found.");
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }

            return RedirectToPage("/YourProfile");
        }

        public IActionResult OnPostCancelAssignment(int assignmentId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("UPDATE Assignments SET BookedByUserId = NULL WHERE AssignmentId = @AssignmentId AND BookedByUserId = @UserId", connection);
                    command.Parameters.AddWithValue("@UserId", loggedInUserId);
                    command.Parameters.AddWithValue("@AssignmentId", assignmentId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Assignment canceled successfully.");
                        return RedirectToPage("/YourProfile");
                    }
                    else
                    {
                        Console.WriteLine("No rows affected.");
                        return Page();  // Return to page with error message
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

        public List<Assignment> GetAssignmentsForUser(int userId)
        {
            var assignments = new List<Assignment>();
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT AssignmentId, PetId, StartDate, Title, City, EndDate FROM Assignments WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assignments.Add(new Assignment
                        {
                            AssignmentId = reader.GetInt32(0),
                            PetId = reader.GetInt32(1),
                            StartDate = reader.GetDateTime(2),
                            Title = reader.GetString(3),
                            City = reader.GetString(4),
                            EndDate = reader.GetDateTime(5)
                        });
                    }
                }
            }
            return assignments;
        }

        public List<Assignment> GetBookedAssignmentsForUser(int userId)
        {
            var assignments = new List<Assignment>();
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand(@"
                    SELECT a.AssignmentId, a.UserId, a.PetId, a.StartDate, a.EndDate, a.TaskType,
                           a.FeedingSchedule, a.FoodAmount, a.Title, a.City, a.Comments, a.Payment,
                           p.PictureUrl, p.Name AS PetName, p.Species, u.Username AS UserName
                    FROM Assignments a
                    JOIN Pets p ON a.PetId = p.PetId
                    JOIN Users u ON a.UserId = u.UserId
                    WHERE a.BookedByUserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assignments.Add(new Assignment
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
                        });
                    }
                }
            }
            return assignments;
        }

        private User GetUserDetails(string username)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT UserId, Username, Email, FullName, CreatedAt, LastLogin FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(0), // Assume UserID cannot be null as it's likely a primary key
                            Username = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                            FullName = reader.IsDBNull(3) ? null : reader.GetString(3),
                        };
                    }
                }
            }
            return null; // or handle as needed if no user is found
        }

        private List<Pets> GetPetsForUser(int userId)
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
