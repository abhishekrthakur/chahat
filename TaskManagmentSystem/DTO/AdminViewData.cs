using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.DTO
{
    public class AdminViewData
    {
        public int TotalTask {  get; set; }
        public int Completed {  get; set; }
        public int ToDo {  get; set; }
        public int Inprogress {  get; set; }
        public int Blocked {  get; set; }
        public int QA {  get; set; }
        public List<Tasks> AssignedToMe { get; set; }
        public List<Tasks> AllTasks { get; set; }
        public List<Tasks> Done { get; set; }
        public List<Tasks> EarlyDue { get; set; }
        public List<Tasks> LateDue { get; set; }
        public List<Tasks> ExceededDueDate { get; set; }
    }
}
