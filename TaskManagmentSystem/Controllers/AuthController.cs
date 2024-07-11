using Microsoft.AspNetCore.Mvc;
using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.Controllers
{
    public class AuthController : Controller
    {
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
            // add auth repo
            return View("~/Views/AuthView/Login.cshtml");
        }

        public IActionResult RegisterUser(User user)
        {
            // add register repo
            return View("~/Views/AuthView/Login.cshtml");
        }
    }
}
