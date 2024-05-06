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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (SqlConnection connection = DB_Connection.GetConnection())
                {
                    if (connection != null)
                    {
                        // SQL query to insert data into the Pets table
                        string query = "INSERT INTO Pets (Name, Species, Breed, Age, Info, OwnerName) VALUES (@Name, @Species, @Breed, @Age, @Info)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Parameters
                            command.Parameters.AddWithValue("@Name", Pet.Name);
                            command.Parameters.AddWithValue("@Species", Pet.Species);
                            command.Parameters.AddWithValue("@Breed", Pet.Breed);
                            command.Parameters.AddWithValue("@Age", Pet.Age);
                            command.Parameters.AddWithValue("@Info", Pet.Info);

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
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Page();
            }
        }
    }
}
