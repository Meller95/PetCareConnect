using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.Data.SqlClient;

namespace PetCareConnect.Pages
{
    public class RegisterPetModel : PageModel
    {
        [BindProperty]
        public Pets Pet { get; set; }

        public void OnGet()
        {
            Pet = new Pets();
        }

        public IActionResult OnPost()
        {
            // Retrieve the logged-in user's ID from the session
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return Unauthorized();  // Simple unauthorized response
            }


            // Set the OwnerId of the pet to the logged-in user's ID
            Pet.OwnerId = loggedInUserId.Value;

            try
            {
                using (SqlConnection connection = DB_Connection.GetConnection())
                {
                    if (connection != null)
                    {
                        // SQL query to insert data into the Pets table, now including OwnerId
                        string query = "INSERT INTO Pets (Name, Species, Breed, Age, Info, OwnerId) VALUES (@Name, @Species, @Breed, @Age, @Info, @OwnerId)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Parameters including OwnerId
                            command.Parameters.AddWithValue("@Name", Pet.Name);
                            command.Parameters.AddWithValue("@Species", Pet.Species);
                            command.Parameters.AddWithValue("@Breed", Pet.Breed);
                            command.Parameters.AddWithValue("@Age", Pet.Age);
                            command.Parameters.AddWithValue("@Info", Pet.Info);
                            command.Parameters.AddWithValue("@OwnerId", Pet.OwnerId);  // Ensure this column exists in your table

                            // Execute the query
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

                // Redirect to a confirmation page or elsewhere after successful registration
                return RedirectToPage("/YourProfile");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Page();
            }
        }
    }
}
