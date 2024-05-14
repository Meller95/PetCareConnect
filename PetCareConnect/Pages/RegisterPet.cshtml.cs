using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace PetCareConnect.Pages
{
    public class RegisterPetModel : PageModel
    {
        [BindProperty]
        public Pets Pet { get; set; }

        [BindProperty]
        public IFormFile UploadedFile { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return Unauthorized();
            }

            Pet.OwnerId = loggedInUserId.Value;

            if (UploadedFile != null && UploadedFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pets");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(UploadedFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await UploadedFile.CopyToAsync(stream);
                    }
                    Pet.PictureUrl = "images/pets/" + fileName;  // Save relative path for later retrieval
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving file: {ex.Message}");
                    return Page();  // Return to page with error message
                }
            }
            else
            {
                Pet.PictureUrl = null;  // Handle case where no file is uploaded
            }

            Console.WriteLine("Attempting to save to database"); // Debugging line

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    string query = "INSERT INTO Pets (Name, Species, Breed, Age, Info, OwnerId, PictureUrl) VALUES (@Name, @Species, @Breed, @Age, @Info, @OwnerId, @PictureUrl)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Pet.Name);
                        command.Parameters.AddWithValue("@Species", Pet.Species);
                        command.Parameters.AddWithValue("@Breed", Pet.Breed);
                        command.Parameters.AddWithValue("@Age", Pet.Age);
                        command.Parameters.AddWithValue("@Info", Pet.Info);
                        command.Parameters.AddWithValue("@OwnerId", Pet.OwnerId);
                        command.Parameters.AddWithValue("@PictureUrl", Pet.PictureUrl);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/YourProfile");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return Page();  // Return to page with error message
            }
        }



    }
}
