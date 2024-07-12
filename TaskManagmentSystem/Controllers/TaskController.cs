using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagmentSystem.DTO;
using TaskManagmentSystem.Models;
using TaskManagmentSystem.Repositories;

namespace TaskManagmentSystem.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskRepository _taskRepository;
        private readonly INotyfService _toastNotification;
        public TaskController(TaskRepository taskRepository, INotyfService toastNotification)
        {
            _taskRepository = taskRepository;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if(!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var tasklist = new TaskListDTO()
            {
                AssignedToMe = _taskRepository.GetUsersTask(userId.Value),
                TeamMatesTasks = _taskRepository.GetTeamsTask(userId.Value)
            };
            return View("~/Views/Dashboard/GenericDashboard.cshtml", tasklist);
        }

        public async Task<IActionResult> CreateTask()
        {
            var teamViewModel = new TeamViewModel
            {
                Teams = await _taskRepository.GetListofTeams(),
                TeamMembers = new List<TeamMembers>()
            };
            return View("~/Views/Dashboard/AddTask.cshtml", teamViewModel);
        }

        public async Task<IActionResult> AddTask(Tasks task)
        {
            //to-do add repo and add duedate in form
            return View("~/Views/Dashboard/AddTask.cshtml");
        }

        [HttpGet]
        public async Task<JsonResult> GetTeamMembers(int teamId)
        {
            var teamMembers = await _taskRepository.GetListofTeamsMembers(teamId);
            return Json(teamMembers);
        }

    }
}
