using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace PetCareConnect.Pages
{
    public class CreateAssignmentModel : PageModel
    {
        [BindProperty]
        public Assignment Assignment { get; set; }

        public List<Pets> RegisteredPets { get; set; }

        public List<SelectListItem> TaskTypes { get; set; } // Dropdown list for task types

        public void OnGet()
        {
            // Get the current user ID
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");

            

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
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");

            if (LoggedInUserId == null)
            {
                // Handle the case where the user is not authenticated
                return RedirectToPage("/YourProfile", new { area = "Identity" });
            }

            // Insert the assignment into the database
            using (SqlConnection connection = DB_Connection.GetConnection())
            {
                if (connection != null)
                {
                    string query = "INSERT INTO Assignments (UserId, PetId, StartDate, EndDate, TaskType, FeedingSchedule, FoodAmount) VALUES (@UserId, @PetId, @StartDate, @EndDate, @TaskType, @FeedingSchedule, @FoodAmount)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", LoggedInUserId);
                        command.Parameters.AddWithValue("@PetId", Assignment.PetId);
                        command.Parameters.AddWithValue("@StartDate", Assignment.StartDate);
                        command.Parameters.AddWithValue("@EndDate", Assignment.EndDate);
                        command.Parameters.AddWithValue("@TaskType", Assignment.TaskType);
                        command.Parameters.AddWithValue("@FeedingSchedule", Assignment.FeedingSchedule);
                        command.Parameters.AddWithValue("@FoodAmount", Assignment.FoodAmount);

                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Connection failed
                    Console.WriteLine("Unable to connect to the database.");
                    return Page();
                }
            }

            return RedirectToPage("/YourProfile");
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
