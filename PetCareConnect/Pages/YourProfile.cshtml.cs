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

        public List<Assignment> GetAssignmentsForUser(int userId)
        {
            var assignments = new List<Assignment>();
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT AssignmentId, PetId, StartDate FROM Assignments WHERE UserId = @UserId", connection);
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
                            PetId = reader.GetInt32(0), // Assuming 'Id' cannot be null
                            Name = reader.GetString(1), // Assuming 'Name' cannot be null
                            Species = reader.GetString(2), // Assuming 'Species' cannot be null
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
