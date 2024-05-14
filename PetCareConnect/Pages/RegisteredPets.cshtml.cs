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
