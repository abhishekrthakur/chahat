using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagmentSystem.DTO;
using TaskManagmentSystem.Models;
using TaskManagmentSystem.Repositories;

namespace TaskManagmentSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly UserRepository _userRepository;
        private readonly INotyfService _toastNotification;

        public EmployeeController(EmployeeRepository employeeRepository, INotyfService toastNotification, UserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _toastNotification = toastNotification;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> GetAttendancePartial()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var tasklist = new TaskListDTO()
            {
                Attendances = await _employeeRepository.GetAttendanceListByUserId((int)userId)
            };
            return PartialView("~/Views/Dashboard/_EmployeeAttendancePartial.cshtml", tasklist);
        }

        public async Task<IActionResult> GetEmployeeAttendancePartial()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var tasklist = new TaskListDTO()
            {
                Members = await _userRepository.GetAllNonAdminUsers(),
                Attendances = await _employeeRepository.GetAttendanceListByUserId((int)userId)
            };
            return PartialView("~/Views/Dashboard/_AdminAttendancePartial.cshtml", tasklist);
        }
        
        public async Task<JsonResult> CheckIn(int userId)
        {
            var attendace = new Attendance();
            attendace.UserId = userId;
            attendace.Date = DateTime.Now.Date;
            attendace.Status = "Pending";
            attendace.Day = DateTime.Now.DayOfWeek.ToString();
            attendace.InTime = DateTime.Now;

            List<Attendance> attendanceList = new List<Attendance>();
            var isAdded = await _employeeRepository.AddAttendance(attendace);
            if(isAdded)
            {
                attendanceList = await _employeeRepository.GetAttendanceListByUserId(userId);
            }
            return Json(attendanceList);
        }

        public async Task<JsonResult> CheckOut(int userId)
        {
            List<Attendance> attendanceList = new List<Attendance>();
            var userAttendance = await _employeeRepository.getAttendanceByDate(userId, DateTime.Now.Date);
            if(userAttendance != null)
            {
                userAttendance.OutTIme = DateTime.Now;
                TimeSpan totalTime = userAttendance.OutTIme - userAttendance.InTime;
                userAttendance.TotalTime = totalTime;
                var isUpdated = await _employeeRepository.UpdateAttendance(userAttendance);
                if(isUpdated)
                {
                    var attendanceListUpdated = await _employeeRepository.GetAttendanceListByUserId(userId);
                    attendanceList = attendanceListUpdated;
                }
            }
            return Json(attendanceList);
        }

        public async Task<JsonResult> GetAttendanceByUser(int userId)
        {
            var attendance = await _employeeRepository.GetAttendanceListByUserId(userId);
            return Json(attendance);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitApprovedAttendance([FromBody] SubmitAttendanceRequest request)
        {
            foreach (var item in request.ApprovedIds)
            {
                var attendance = await _employeeRepository.getAttendanceById(item);
                if (attendance.Status == "Pending")
                {
                    attendance.Status = "Approved";
                    var isUpdated = await _employeeRepository.UpdateAttendance(attendance);
                }
            }
            var attendanceList = await _employeeRepository.GetAttendanceListByUserId(request.UserId);
            return Json(attendanceList);
        }
    }
}
