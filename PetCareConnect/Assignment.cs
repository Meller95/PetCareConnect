public class Assignment
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public int PetId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TaskType { get; set; }
    public string FeedingSchedule { get; set; }
    public int FoodAmount { get; set; }
}