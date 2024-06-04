using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System;
using System.Collections.Generic;

namespace PetCareConnect.Pages
{
    public class UserProfileModel : PageModel
    {
        public User User { get; set; } = new User(); // Initialized to avoid null reference
        public Assignment Assignment { get; set; } // Initialized to avoid null reference

        public IActionResult OnGet(int userId, int assignmentId)
        {
            var currentUser = HttpContext.Session.GetInt32("UserId");

            if (currentUser == null)
            {
                return RedirectToPage("/SignIn");
            }

            User = GetUserDetails(userId);

            if (User != null)
            {
                Assignment = GetAssignmentDetails(assignmentId);
                Assignment.PictureUrl = GetPetPictureUrl(Assignment.PetId);
            }
            else
            {
                return RedirectToPage("/SignIn");
            }

            return Page();
        }

        private User GetUserDetails(int userId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT * FROM Users WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            ProfilePictureUrl = reader["ProfilePictureUrl"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        private Assignment GetAssignmentDetails(int assignmentId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand(@"
                    SELECT a.AssignmentId, a.UserId, a.PetId, a.StartDate, a.EndDate, a.TaskType,
                           a.FeedingSchedule, a.FoodAmount, a.Title, a.City, a.Comments, a.Payment,
                           p.Name AS PetName, p.Species, u.Username AS UserName, 
                           ISNULL(a.BookingConfirmed, 0) AS BookingConfirmed
                    FROM Assignments a
                    JOIN Pets p ON a.PetId = p.PetId
                    JOIN Users u ON a.UserId = u.UserId
                    WHERE a.AssignmentId = @AssignmentId", connection);
                command.Parameters.AddWithValue("@AssignmentId", assignmentId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Assignment
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
                            PetName = reader.GetString(12),
                            Species = reader.GetString(13),
                            UserName = reader.GetString(14),
                            BookingConfirmed = reader.GetBoolean(15)
                        };
                    }
                }
            }
            return null;
        }

        private string GetPetPictureUrl(int petId)
        {
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT PictureUrl FROM Pets WHERE PetId = @PetId", connection);
                command.Parameters.AddWithValue("@PetId", petId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader["PictureUrl"].ToString();
                    }
                }
            }
            return null;
        }
    }
}
