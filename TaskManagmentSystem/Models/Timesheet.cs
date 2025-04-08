namespace TaskManagmentSystem.Models
{
    public class Timesheet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Project { get; set; }
        public string Task { get; set; }
        public string Hours { get; set; }
        public string Description { get; set; }
    }
}
