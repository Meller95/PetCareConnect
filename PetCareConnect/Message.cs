﻿namespace PetCareConnect.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }

        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
