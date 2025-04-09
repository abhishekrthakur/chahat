﻿using TaskManagmentSystem.Models;
namespace TaskManagmentSystem.DTO
{
    public class TaskListDTO
    {
        public List<Teams> Teams { get; set; }
        public List<User> Members { get; set; }
        public List<Tasks> AssignedToMe { get; set;}
        public List<Tasks> TeamMatesTasks { get; set; }
        public List<Attendance> Attendances { get; set; }
        public List<Timesheet> Timesheets { get; set; }
        public string Username  { get; set; }
    }
}
