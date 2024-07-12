using System.Linq;
using TaskManagmentSystem.Data;
using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.Repositories
{
    public class TaskRepository
    {
        private readonly TaskManagmentDBContext _dbContext;
        public TaskRepository(TaskManagmentDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Tasks> GetUsersTask(int userId)
        {
            var tasks = _dbContext.Tasks.Where(x => x.AssignedTo == userId).ToList();

            return tasks;
        }

        public List<Tasks> GetTeamsTask(int userId)
        {
            var tasks = new List<Tasks>();
            var userTeams = _dbContext.TeamMembers.Where(x => x.UserId == userId).ToList();
            if (userTeams.Count != 0)
            {
                var teams = userTeams.Select(x => x.UserId).ToList();
                tasks = _dbContext.Tasks.Where(x => teams.Contains(x.TeamId) && x.AssignedTo != userId).ToList();
            }
           
            return tasks;
        }
    }
}
