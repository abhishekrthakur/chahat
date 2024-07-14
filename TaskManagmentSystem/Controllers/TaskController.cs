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

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var tasklist = new TaskListDTO()
            {
                Members = await _userRepository.GetAllUsers(),
                Teams = await _taskRepository.GetListofTeams(),
                AssignedToMe = _taskRepository.GetUsersTask(userId.Value),
                TeamMatesTasks = _taskRepository.GetTeamsTask(userId.Value)
            };
            return View("~/Views/Dashboard/GenericDashboard.cshtml", tasklist);
        }

        public async Task<IActionResult> AdminView()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var admindata = await _taskRepository.GetAdminViewData(userId.Value);
            admindata.Members = await _userRepository.GetAllUsers();
            return View("~/Views/Dashboard/AdminDashboard.cshtml", admindata);
        }

        public async Task<IActionResult> DetailView(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var user = _userRepository.GetUserByUserId(userId.Value);
            var taskDetails = await _taskRepository.GetTaskDetailViewData(id);
            taskDetails.UserRole = user.UserRole;
            return View("~/Views/Tasks/TaskView.cshtml", taskDetails);
        }

        public async Task<IActionResult> CreateTask()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var user = _userRepository.GetUserByUserId(userId.Value);
            var teamViewModel = new TeamViewModel
            {
                UserRole = user.UserRole,
                Teams = await _taskRepository.GetListofTeams(),
                TeamMembers = new List<TeamMembers>()
            };
            return View("~/Views/Dashboard/AddTask.cshtml", teamViewModel);
        }

        public async Task<IActionResult> AddTask(IFormFile file, Tasks task)
        {
            var assingedTo = _userRepository.GetUserByUserId(task.AssignedTo);

            //fetching creator id from session
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

            task.Status = TasksStatus.ToDo;

            var result = await _taskRepository.AddTask(task);
            //uploading attachment
            if(file != null)
              await UploadAssignment(file, task.TaskId);
            if (result != null)
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

        public async Task<IActionResult> Download(int id)
        {
            var document = await _taskRepository.GetAttachmentById(id);
            if (document != null)
            {
                return File(document.DocData, document.DocType, document.FileName);
            }
            return NotFound();
        }

        public async Task<IActionResult> AddNotes(int TaskId, string NoteText)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var user = _userRepository.GetUserByUserId(userId.Value);

            var noteDetail = new Notes()
            {
                NoteText = NoteText,
                TaskId = TaskId,
                CreatedUser = user.Username,
                CreatedBy = user.UserId,
                CreatedDate = DateTime.Now,
            };

            var result = await _taskRepository.AddNotes(noteDetail);
            if (result)
                _toastNotification.Success("Note Added Successfully !!");
            else
                _toastNotification.Error("Something went wrong !! Try Again");

            var taskDetails = await _taskRepository.GetTaskDetailViewData(TaskId);
            taskDetails.UserRole = user.UserRole;
            return View("~/Views/Tasks/TaskView.cshtml", taskDetails);
        }

        public async Task<IActionResult> UpdateStatus(int TaskId, string Status)
        {
            var result = await _taskRepository.UpdateStatus(TaskId, Status);
            if (result)
                return Ok();

            return BadRequest();
        }

        public async Task<IActionResult> UpdateTask(Tasks tasks)
        {
            var taskDetail = await _taskRepository.GetTaskDetail(tasks.TaskId);
            taskDetail.Title = tasks.Title;
            taskDetail.Description = tasks.Description;
            taskDetail.DueDate = tasks.DueDate;
            if (taskDetail.AssignedTo != tasks.AssignedTo)
            {
                var assingedTo = _userRepository.GetUserByUserId(tasks.AssignedTo);
                taskDetail.AssignedTo = assingedTo.UserId;
                taskDetail.AssignedUser = assingedTo.Username;
            }

            var result = await _taskRepository.UpdateTask(taskDetail);
            if (result)
            {
                _toastNotification.Success("Task Updated SucessFully !!");
            }
            else
            {
                _toastNotification.Success("An Error Occurred while Updating !!");
            }

            return RedirectToAction("DetailView", new { id = tasks.TaskId });
        }

        public async Task<IActionResult> AddAttachments(IFormFile file, int taskId)
        {
            await UploadAssignment(file, taskId);
            return RedirectToAction("DetailView", new { id = taskId });
        }

        public async Task<IActionResult> AddTeam(List<int> SelectedMemberIds, string teamName)
        {
            if (SelectedMemberIds != null && SelectedMemberIds.Count > 0)
            {
                var team = await _taskRepository.AddTeam(teamName);
                var result = false;
                if (team != null)
                {
                    result = await _taskRepository.AddTeamMember(team, SelectedMemberIds);
                    if (result)
                    {
                        _toastNotification.Success("Team Added Successfully");
                    }
                }
                if (team == null || !result)
                {
                    _toastNotification.Error("An Error Occurred !! Try Again");
                }
            }
            else
            {
                _toastNotification.Warning("Please Select a Team Member");
            }
            string role = HttpContext.Session.GetString("Role")!;

            if (string.IsNullOrEmpty(role))
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            if (role != "CompanyAdmin")
                return RedirectToAction("Index");
            else
                return RedirectToAction("AdminView");
        }

        public async Task<IActionResult> AddMember(List<int> SelectedMemberIds, int TeamId)
        {
            if (SelectedMemberIds != null && SelectedMemberIds.Count > 0)
            {
                var team = await _taskRepository.GetTeam(TeamId);
                var result = false;
                if (team != null)
                {
                    result = await _taskRepository.AddTeamMember(team, SelectedMemberIds);
                    if (result)
                    {
                        _toastNotification.Success("Members Added Successfully");
                    }
                }
                if (team == null || !result)
                {
                    _toastNotification.Error("An Error Occurred !! Try Again");
                }
            }
            else
            {
                _toastNotification.Warning("Please Select a Team Member");
            }
            string role = HttpContext.Session.GetString("Role")!;

            if (string.IsNullOrEmpty(role))
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            if (role != "CompanyAdmin")
                return RedirectToAction("Index");
            else
                return RedirectToAction("AdminView");
        }

        [HttpGet]
        public async Task<JsonResult> GetTeamMembers(int teamId)
        {
            var teamMembers = await _taskRepository.GetListofTeamsMembers(teamId);
            return Json(teamMembers);
        }

        [HttpGet]
        public async Task<JsonResult> GetNewMember(int teamId)
        {
            var teamMembers = await _taskRepository.GetListofNewTeamsMembers(teamId);
            return Json(teamMembers);
        }

        public async Task<IActionResult> UploadAssignment(IFormFile file, int taskId)
        {
            //fetching creator id from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _toastNotification.Warning("Session Expire, Login Again !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            var createdBy = _userRepository.GetUserByUserId(userId.Value);
            var result = false;
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    var document = new Attachments
                    {
                        TaskId = taskId,
                        DocData = memoryStream.ToArray(),
                        DocType = file.ContentType,
                        FileName = file.FileName,
                        UploadedBy = createdBy.UserId,
                        UploadedUser = createdBy.Username,
                        UploadDate = DateTime.Now
                    };
                    result = await _taskRepository.AddAttachment(document);
                }
            }
            if (!result)
            {
                _toastNotification.Error("Error while Uploading Attachment !!");
                return BadRequest();
            }
            _toastNotification.Success("Attachment Uploaded Successfully !!");
            return Ok();
        }
    }
}
