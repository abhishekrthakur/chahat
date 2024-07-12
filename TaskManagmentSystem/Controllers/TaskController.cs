using Microsoft.AspNetCore.Mvc;

namespace TaskManagmentSystem.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
