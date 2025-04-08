using Microsoft.EntityFrameworkCore;
using TaskManagmentSystem.Data;
using TaskManagmentSystem.Migrations;
using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.Repositories
{
    public class EmployeeRepository
    {
        private readonly TaskManagmentDBContext _dbContext;
        public EmployeeRepository(TaskManagmentDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Attendance>> GetAttendanceListByUserId(int userId)
        {
            var attendanceList = await _dbContext.Attendances
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (attendanceList.Count == 0)
            {
                var now = DateTime.Now;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var yesterday = now.Date.AddDays(-1);

                List<Attendance> generatedAttendances = new List<Attendance>();

                for (var date = firstDayOfMonth; date <= yesterday; date = date.AddDays(1))
                {
                    DateTime inTime = date.AddHours(9);  // e.g., 9 AM
                    DateTime outTime = date.AddHours(17); // e.g., 5 PM
                    TimeSpan totalTime = outTime - inTime;

                    var attendance = new Attendance();
                    attendance.UserId = userId;
                    attendance.Date = date;
                    attendance.InTime = inTime;
                    attendance.OutTIme = outTime;
                    attendance.TotalTime = totalTime;
                    attendance.Day = date.DayOfWeek.ToString();
                    attendance.Status = "Pending";
                    generatedAttendances.Add(attendance);
                }

                await _dbContext.Attendances.AddRangeAsync(generatedAttendances);
                await _dbContext.SaveChangesAsync();

                attendanceList = generatedAttendances;
            }
            else
            {
                var attendanceListByUserId = await _dbContext.Attendances.Where(x => x.UserId == userId).ToListAsync();
                attendanceList = attendanceListByUserId;
            }

            attendanceList = attendanceList.OrderByDescending(x => x.Date).ToList();

            return attendanceList;
        }

        public async Task<bool> AddAttendance(Attendance attendance)
        {
            _dbContext.Attendances.Add(attendance);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Attendance> getAttendanceByDate(int userId, DateTime date)
        {
            return await _dbContext.Attendances.Where(x => x.UserId == userId && x.Date == date.Date).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAttendance(Attendance attendance)
        {
            _dbContext.Attendances.Update(attendance);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<Timesheet>> GetUserTimesheetList(int userId)
        {
            return await _dbContext.Timesheets.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
