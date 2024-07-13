using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection.Metadata;
using TaskManagmentSystem.Data;
using TaskManagmentSystem.DTO;
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

        public async Task<List<Teams>> GetListofTeams()
        {
            return await _dbContext.Teams.ToListAsync();
        }

        public async Task<List<TeamMembers>> GetListofTeamsMembers(int teamId)
        {
            return await _dbContext.TeamMembers.Where(x=>x.TeamId == teamId).ToListAsync();
        }

        public async Task<Tasks> AddTask(Tasks task)
        {
            try
            {
                await _dbContext.Tasks.AddAsync(task);
                await _dbContext.SaveChangesAsync();
                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public async Task<bool> AddAttachment(Attachments document)
        {
            try
            {
                await _dbContext.Attachments.AddAsync(document);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Tasks> GetTaskDetail(int id)
        {
            try
            {
                var taskDetails = await _dbContext.Tasks.FirstOrDefaultAsync(x=>x.TaskId==id);
                if (taskDetails is null)
                  return null!;
                return taskDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
        public async Task<List<Attachments>> GetAttachmentsDetail(int id)
        {
            try
            {
                var attachments = await _dbContext.Attachments.Where(x => x.TaskId == id).ToListAsync();
                if (attachments is null || attachments.Count == 0 )
                    return [];
                return attachments;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        public async Task<List<Notes>> GetNotesDetail(int id)
        {
            try
            {
                var notes = await _dbContext.Notes.Where(x => x.TaskId == id).ToListAsync();
                if (notes is null || notes.Count == 0)
                    return [];
                return [.. notes.OrderByDescending(x=>x.CreatedDate)];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        public async Task<Attachments> GetAttachmentById(int id)
        {
            try
            {
                var attachments = await _dbContext.Attachments.FirstOrDefaultAsync(x => x.AttachmentId == id);
                if (attachments is null)
                    return null!;
                return attachments;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public async Task<bool> AddNotes(Notes note)
        {
            try
            {
                await _dbContext.Notes.AddAsync(note);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<TaskDetailDTO> GetTaskDetailViewData(int id)
        {
            var task = await GetTaskDetail(id);
            var taskDetails = new TaskDetailDTO()
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                AssignedTo = task.AssignedTo,
                TeamId = task.TeamId,
                AssignedUser = task.AssignedUser,
                CreatedBy = task.CreatedBy,
                CreatedUser = task.CreatedUser,
                DueDate = task.DueDate,
                Status = task.Status,
                Attachments = await GetAttachmentsDetail(id),
                Notes = await GetNotesDetail(id)
            };
            return taskDetails;
        }

        public async Task<bool> UpdateTask(int id,string status)
        {
            try
            {
                var task = await GetTaskDetail(id);
                if(task != null)
                {
                    task.Status = status;
                    _dbContext.Update(task);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
