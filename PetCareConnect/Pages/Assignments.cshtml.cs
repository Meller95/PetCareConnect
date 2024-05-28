using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Collections.Generic;

namespace PetCareConnect.Pages
{
    public class AssignmentsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchCity { get; set; }

        public List<Assignment> Assignments { get; set; }

        public void OnGet()
        {
            Assignments = new List<Assignment>();

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    string query = @"
                    SELECT a.AssignmentId, a.UserId, a.PetId, a.StartDate, a.EndDate, a.TaskType,
                           a.FeedingSchedule, a.FoodAmount, a.Title, a.City, a.Comments, a.Payment,
                           p.PictureUrl, p.Name AS PetName, p.Species, u.Username AS UserName
                    FROM Assignments a
                    JOIN Pets p ON a.PetId = p.PetId
                    JOIN Users u ON a.UserId = u.UserId
                    WHERE (@SearchCity IS NULL OR a.City LIKE '%' + @SearchCity + '%')
                    AND a.BookedByUserId IS NULL"; // Only show assignments that are not booked

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchCity", string.IsNullOrEmpty(SearchCity) ? (object)DBNull.Value : SearchCity);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var assignment = new Assignment
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
                                };
                                // Log the assignment details to debug
                                Console.WriteLine($"AssignmentId: {assignment.AssignmentId}, PetName: {assignment.PetName}, PictureUrl: {assignment.PictureUrl}");
                                Assignments.Add(assignment);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
            }
        }


        public IActionResult OnPostBookAssignment(int assignmentId)
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
                    var command = new SqlCommand("UPDATE Assignments SET BookedByUserId = @UserId WHERE AssignmentId = @AssignmentId", connection);
                    command.Parameters.AddWithValue("@UserId", loggedInUserId);
                    command.Parameters.AddWithValue("@AssignmentId", assignmentId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Assignment booked successfully.");
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
    }
}
