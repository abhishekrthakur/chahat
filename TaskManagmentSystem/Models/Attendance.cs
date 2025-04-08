namespace TaskManagmentSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTIme { get; set; }
        public TimeSpan TotalTime { get; set; }
        public string Day { get; set; }
        public string Status { get; set; }
    }
}
