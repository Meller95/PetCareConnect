using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System.Collections.Generic;

namespace PetCareConnect.Pages
{
    public class AssignmentsModel : PageModel
    {
        public List<Assignment> Assignments { get; set; }

        public void OnGet()
        {
            Assignments = new List<Assignment>();

            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    string query = @"
                        SELECT a.AssignmentId, a.StartDate, a.EndDate, a.TaskType, a.FeedingSchedule, a.FoodAmount, p.Name AS PetName, u.Username AS UserName
                        FROM Assignments a
                        JOIN Pets p ON a.PetId = p.PetId
                        JOIN Users u ON a.UserId = u.UserId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var assignment = new Assignment
                                {
                                    AssignmentId = reader.GetInt32(0),
                                    StartDate = reader.GetDateTime(1),
                                    EndDate = reader.GetDateTime(2),
                                    TaskType = reader.GetString(3),
                                    FeedingSchedule = reader.GetString(4),
                                    FoodAmount = reader.GetString(5),
                                    PetName = reader.GetString(6),
                                    UserName = reader.GetString(7)
                                };
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
    }
}
