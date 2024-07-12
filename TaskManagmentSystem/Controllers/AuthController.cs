using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TaskManagmentSystem.Constants;
using TaskManagmentSystem.Models;
using TaskManagmentSystem.Repositories;

namespace TaskManagmentSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly TaskRepository _taskRepository;
        private readonly INotyfService _toastNotification;
        public AuthController(UserRepository userRepository, TaskRepository taskRepository, INotyfService toastNotification)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _toastNotification = toastNotification;
        }
        public IActionResult Login()
        {
            return View("~/Views/AuthView/Login.cshtml");
        }

        public IActionResult Register()
        {
            return View("~/Views/AuthView/Register.cshtml");
        }

        public IActionResult AuthenticateUser(string userName, string password)
        {
            var user = _userRepository.GetUserByUserName(userName);
            if(user == null)
            {
                _toastNotification.Error("No such User exist !!!");
                return View("~/Views/AuthView/Login.cshtml");
            }
            if(user != null && user.Password.ToLower() == password.ToLower())
            {
                var task = _taskRepository.GetUsersTask(user.UserId);
                _toastNotification.Success("Logged In successfully!!");
                return View("~/Views/Dashboard/GenericDashboard.cshtml", task);
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
    }
}
