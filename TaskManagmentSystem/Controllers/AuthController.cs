using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TaskManagmentSystem.Models;
using TaskManagmentSystem.Repositories;

namespace TaskManagmentSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly INotyfService _toastNotification;
        public AuthController(UserRepository userRepository, INotyfService toastNotification)
        {
            _userRepository = userRepository;
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
                _toastNotification.Success("rebrtb");
            }
            return View("~/Views/AuthView/Login.cshtml");
        }

        public IActionResult RegisterUser(User user)
        {
            // add register repo
            return View("~/Views/AuthView/Login.cshtml");
        }
    }
}
