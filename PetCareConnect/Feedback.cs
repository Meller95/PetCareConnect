using System;
using System.ComponentModel.DataAnnotations;

namespace PetCareConnect.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime FeedbackDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string FeedbackText { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
