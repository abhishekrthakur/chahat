using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.DTO
{
    public class TaskListDTO
    {
        public List<Tasks> AssignedToMe { get; set;}
        public List<Tasks> TeamMatesTasks { get; set; }
    }
}
