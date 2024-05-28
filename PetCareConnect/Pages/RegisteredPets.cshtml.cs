using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace PetCareConnect.Pages
{
    public class RegisteredPetsModel : PageModel
    {
        public List<Pets> RegisteredPets { get; set; }

        public void OnGet()
        {
            // Get the current user's ID
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");

            // Load user's registered pets from the database
            RegisteredPets = GetPetsForUser((int)LoggedInUserId);
        }

        public IActionResult OnPostDeletePet(int id)
        {
            // Delete pet from the database
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    var command = new SqlCommand("DELETE FROM Pets WHERE PetId = @PetId", connection);
                    command.Parameters.AddWithValue("@PetId", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return RedirectToPage();
        }

        private List<Pets> GetPetsForUser(int userId)
        {
            var pets = new List<Pets>();
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT PetId, Name, Species, Breed, Age, Info, PictureUrl FROM Pets WHERE OwnerId = @OwnerId", connection);
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
                            Info = reader.IsDBNull(5) ? null : reader.GetString(5),
                            PictureUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                        });
                    }
                }
            }
            return pets;
        }
    }
}
