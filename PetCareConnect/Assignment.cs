public class Assignment
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public int PetId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TaskType { get; set; }
    public string FeedingSchedule { get; set; }
    public string FoodAmount { get; set; }
    public string Title { get; set; }
    public string City { get; set; }
    public string Comments { get; set; }
    public decimal Payment { get; set; }
    public string PictureUrl { get; set; }
    public string PetName { get; set; }
    public string Species { get; set; }
    public string UserName { get; set; }
    public bool BookingConfirmed { get; set; }
    public int? BookedByUserId { get; set; }
    public string BookedByUsername { get; set; }
    public string BookedByUserProfilePictureUrl { get; set; }
}
