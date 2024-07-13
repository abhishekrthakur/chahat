using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.DTO
{
    public class TeamViewModel
    {
        public string UserRole { get; set; }
        public int SelectedTeamId { get; set; }
        public List<Teams> Teams { get; set; }
        public List<TeamMembers> TeamMembers { get; set; }
    }
}
