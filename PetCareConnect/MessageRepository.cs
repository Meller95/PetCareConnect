using Microsoft.Data.SqlClient;
using PetCareConnect.Models;
using System;
using System.Collections.Generic;

namespace PetCareConnect.Data
{
    public class MessageRepository
    {
        public List<Message> GetMessages(int currentUserId)
        {
            var messages = new List<Message>();

            using (var connection = DB_Connection.GetConnection())
            {
                var query = @"
            SELECT m.MessageId, m.SenderId, m.ReceiverId, m.Content, m.Timestamp,
                   s.Username AS SenderUsername, r.Username AS ReceiverUsername
            FROM Messages m
            JOIN Users s ON m.SenderId = s.UserId
            JOIN Users r ON m.ReceiverId = r.UserId
            WHERE m.SenderId = @CurrentUserId OR m.ReceiverId = @CurrentUserId
            ORDER BY m.Timestamp DESC"; // Ordering messages by timestamp in descending order

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CurrentUserId", currentUserId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new Message
                            {
                                MessageId = reader.GetInt32(0),
                                SenderId = reader.GetInt32(1),
                                ReceiverId = reader.GetInt32(2),
                                Content = reader.GetString(3),
                                Timestamp = reader.GetDateTime(4),
                                SenderUsername = reader.GetString(5),
                                ReceiverUsername = reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return messages;
        }

        public void SendMessage(string senderUsername, string receiverUsername, string content)
        {
            // Retrieve the sender's ID based on the provided username
            int senderId = GetUserIdByUsername(senderUsername);

            // Retrieve the receiver's ID based on the provided username
            int receiverId = GetUserIdByUsername(receiverUsername);

            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand(@"
        INSERT INTO Messages (SenderId, ReceiverId, Content, Timestamp)
        VALUES (@SenderId, @ReceiverId, @Content, @Timestamp)", connection);

                command.Parameters.AddWithValue("@SenderId", senderId);
                command.Parameters.AddWithValue("@ReceiverId", receiverId);
                command.Parameters.AddWithValue("@Content", content);
                command.Parameters.AddWithValue("@Timestamp", DateTime.Now); // Assuming you want to store the current timestamp
                command.ExecuteNonQuery();
            }
        }

        // Helper method to get user ID by username
        private int GetUserIdByUsername(string username)
        {
            int userId = 0;
            using (var connection = DB_Connection.GetConnection())
            {
                var command = new SqlCommand("SELECT UserId FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                // Execute scalar query to get the user ID
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out userId))
                {
                    return userId;
                }
            }
            return userId;
        }



        private string GetUsernameById(int userId)
        {
            string username = null;

            using (var connection = DB_Connection.GetConnection())
            {
                try
                {
                    // Prepare SQL command to fetch username by user ID
                    string query = "SELECT Username FROM Users WHERE UserId = @UserId";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        // Execute the command and retrieve the username
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            username = result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    Console.WriteLine("Error retrieving username: " + ex.Message);
                }
            }

            return username;
        }

    }
}
