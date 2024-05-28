using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;

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

            if (LoggedInUserId == null)
            {
                // Redirect to login page or show an error
                RedirectToPage("/Login");
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
        }

        public IActionResult OnPost()
        {
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");

            if (LoggedInUserId == null)
            {
                // Handle the case where the user is not authenticated
                return RedirectToPage("/YourProfile", new { area = "Identity" });
            }

            Assignment.UserId = (int)LoggedInUserId; // Set the UserId in Assignment

            // Insert the assignment into the database
            try
            {
                using (SqlConnection connection = DB_Connection.GetConnection())
                {
                    if (connection != null)
                    {
                        string query = "INSERT INTO Assignments (UserId, PetId, StartDate, EndDate, TaskType, FeedingSchedule, FoodAmount, Title, City, Comments, Payment) VALUES (@UserId, @PetId, @StartDate, @EndDate, @TaskType, @FeedingSchedule, @FoodAmount, @Title, @City, @Comments, @Payment)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", Assignment.UserId);
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

                            command.ExecuteNonQuery();
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
