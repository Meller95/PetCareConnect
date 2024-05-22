using System;
using System.ComponentModel.DataAnnotations;

namespace PetCareConnect.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }

        public int UserId { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(100)]
        public string TaskType { get; set; }

        [Required]
        [StringLength(100)]
        public string FeedingSchedule { get; set; }

        [Required]
        public string FoodAmount { get; set; }

        public string PetName { get; set; } // Additional property for Pet Name
        public string UserName { get; set; } // Additional property for User Name
    }
}
