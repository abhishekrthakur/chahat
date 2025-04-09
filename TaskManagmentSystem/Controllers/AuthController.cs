using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TaskManagmentSystem.Constants;
using TaskManagmentSystem.DTO;
using TaskManagmentSystem.Models;
using TaskManagmentSystem.Repositories;

namespace TaskManagmentSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly TaskRepository _taskRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly INotyfService _toastNotification;
        public AuthController(UserRepository userRepository, TaskRepository taskRepository, INotyfService toastNotification, EmployeeRepository employeeRepository)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _toastNotification = toastNotification;
            _employeeRepository = employeeRepository;
        }
        public IActionResult Login()
        {
            return View("~/Views/AuthView/Login.cshtml");
        }

        public IActionResult Register()
        {
            return View("~/Views/AuthView/Register.cshtml");
        }

        public async Task<IActionResult> AuthenticateUser(string userName, string password)
        {
            var user = _userRepository.GetUserByUserName(userName);
            if(user == null)
            {
                _toastNotification.Error("No such User exist !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            if(user != null && user.Password == password)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                if (user.UserRole != null && user.UserRole != UserRoles.CompanyAdmin)
                {
                    HttpContext.Session.SetString("Role", "Team");
                    var tasklist = new TaskListDTO()
                    {
                        Username = user.Username,
                        Members = await _userRepository.GetAllUsers(),
                        Teams = await _taskRepository.GetListofTeams(),
                        AssignedToMe = _taskRepository.GetUsersTask(user.UserId),
                        TeamMatesTasks = _taskRepository.GetTeamsTask(user.UserId),
                        Attendances = await _employeeRepository.GetAttendanceListByUserId(user.UserId)
                    };
                    return View("~/Views/Dashboard/GenericDashboard.cshtml", tasklist);
                }
                else if (user.UserRole != null && user.UserRole == UserRoles.CompanyAdmin)
                {
                    HttpContext.Session.SetString("Role", "CompanyAdmin");
                    var admindata = await _taskRepository.GetAdminViewData(user.UserId);
                    admindata.Members = await _userRepository.GetAllUsers();
                    admindata.Username = user.Username;
                    return View("~/Views/Dashboard/AdminDashboard.cshtml", admindata);
                }
                _toastNotification.Success("Logged In successfully!!");     
            }
            else
            {
                _toastNotification.Error("Incorrect UserName or Password");
            }
            return View("~/Views/AuthView/Login.cshtml");
        }

        public async Task<IActionResult> RegisterUser(User user)
        {
            var userDetails = _userRepository.GetUserByUserName(user.Username);
            if(userDetails != null)
            {
                _toastNotification.Error("userName Already Exits !!");
                return View("~/Views/AuthView/Register.cshtml");
            }

            if (user != null && user.Password.Length < 5)
            {
                _toastNotification.Error("Use a longer Password!!");
                return View("~/Views/AuthView/Register.cshtml");
            }

            user.UserRole = UserRoles.TeamMember;
            var result = await _userRepository.UpdateUser(user);

            if (result)
            {
                _toastNotification.Success("Account Created Successfully !! Please Login");
                return View("~/Views/AuthView/Login.cshtml");
            }
            else
                _toastNotification.Error("An error Occured !! Please Try Again");
            return View("~/Views/AuthView/Register.cshtml");
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var tasklist = new TaskListDTO()
            {
                Members = await _userRepository.GetAllUsers(),
                Teams = await _taskRepository.GetListofTeams(),
                AssignedToMe = _taskRepository.GetUsersTask((int)userId),
                TeamMatesTasks = _taskRepository.GetTeamsTask((int)userId),
                Attendances = await _employeeRepository.GetAttendanceListByUserId((int)userId)
            };
            return View("~/Views/Dashboard/GenericDashboard.cshtml", tasklist);
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _userRepository.GetUserByUserId((int)userId);
            var admindata = await _taskRepository.GetAdminViewData(user.UserId);
            admindata.Members = await _userRepository.GetAllUsers();
            return View("~/Views/Dashboard/AdminDashboard.cshtml", admindata);
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("UserId");
            return View("~/Views/AuthView/Login.cshtml");
        }

    }
}
