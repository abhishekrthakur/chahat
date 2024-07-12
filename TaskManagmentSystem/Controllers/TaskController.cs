using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TaskManagmentSystem.Constants;
using TaskManagmentSystem.DTO;
using TaskManagmentSystem.Models;
using TaskManagmentSystem.Repositories;

namespace TaskManagmentSystem.Controllers
{
    public class TaskController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly TaskRepository _taskRepository;
        private readonly INotyfService _toastNotification;
        public TaskController(TaskRepository taskRepository, INotyfService toastNotification, UserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _toastNotification = toastNotification;
            _userRepository = userRepository;
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

            var assingedTo = _userRepository.GetUserByUserId(task.AssignedTo);

            //fetch creator id from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var createdBy = _userRepository.GetUserByUserId(userId.Value);

            task.CreatedUser = createdBy.Username;
            task.CreatedBy = createdBy.UserId;
            task.AssignedUser = assingedTo.Username;

            task.DueDate = DateTime.Now.AddDays(7);
            task.Status = TasksStatus.ToDo;
            var result = await _taskRepository.AddTask(task);
            if(result)
            {
                _toastNotification.Success("Task Created Successfully !!");
                return RedirectToAction("Index");
            }
            else
            {
                _toastNotification.Error("Something went wrong !! Try Again");
                return View("~/Views/Dashboard/AddTask.cshtml");
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetTeamMembers(int teamId)
        {
            var teamMembers = await _taskRepository.GetListofTeamsMembers(teamId);
            return Json(teamMembers);
        }

    }
}
