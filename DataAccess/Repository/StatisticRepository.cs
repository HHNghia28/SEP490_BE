using BusinessObject.DTOs;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly ApplicationDbContext _context;

        public StatisticRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StatisticAttendenceResponse>> GetStatisticAttendance(string schoolYear, int grade = 0, string fromDate = null, string toDate = null)
        {
            var currentDate = DateTime.Now;

            DateTime? parsedFromDate = null;
            DateTime? parsedToDate = null;

            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                var firstDayOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);

                parsedFromDate = firstDayOfWeek;

                parsedToDate = firstDayOfWeek.AddDays(6);
            }
            else
            {
                if (DateTime.TryParseExact(fromDate, new[] { "dd/MM/yyyy", "d/M/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFrom))
                {
                    parsedFromDate = parsedFrom;
                }

                if (DateTime.TryParseExact(toDate, new[] { "dd/MM/yyyy", "d/M/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo))
                {
                    parsedToDate = parsedTo;
                }
            }

            // Fetch schedules and related data
            var classSchedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                .ThenInclude(s => s.Teacher)
                .Include(s => s.Teacher)
                .Where(s => s.Classes.SchoolYear.Name == schoolYear && s.Classes.IsActive && (!parsedFromDate.HasValue || s.Date >= parsedFromDate.Value) &&
                            (!parsedToDate.HasValue || s.Date <= parsedToDate.Value))
                .Select(s => new
                {
                    s.ID,
                    s.Classes.Classroom,
                    TeacherName = s.Classes.Teacher.Username,
                    s.Date,
                    Grade = s.Classes.Classroom.Length >= 2 ? int.Parse(s.Classes.Classroom.Substring(0, 2)) : (int?)null
                })
                .ToListAsync();

            // Filter schedules by grade
            var filteredSchedules = classSchedules
                .Where(s => grade == 0 || s.Grade == grade)
                .ToList();

            // Get the IDs of the filtered schedules
            var scheduleIds = filteredSchedules.Select(s => s.ID).ToList();

            // Fetch attendances for the filtered schedules
            var attendances = await _context.Attendances
                .AsNoTracking()
                .Where(a => scheduleIds.Contains(a.ScheduleID))
                .Select(a => new
                {
                    a.ScheduleID,
                    a.Present,
                    a.Date,
                    a.StudentID
                })
                .ToListAsync();

            // Group and process the data, aggregating by classroom name
            var groupedData = filteredSchedules
                .GroupBy(cs => new
                {
                    cs.Classroom,
                    cs.TeacherName
                })
                .Select(g => new StatisticAttendenceResponse
                {
                    ClassName = g.Key.Classroom,
                    Grade = grade == 0 ? g.Key.Classroom.Substring(0, 2) : grade.ToString(),
                    Teacher = g.Key.TeacherName,
                    NumberOfStudent = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID)).Select(a => a.StudentID).Distinct().Count(),
                    NumberOfPresent = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID) && a.Date <= currentDate && a.Present).Count(),
                    NumberOfAbsent = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID) && a.Date <= currentDate && !a.Present).Count(),
                    NumberOfNotYet = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID) && a.Date > currentDate).Count()
                })
                .OrderBy(g => g.ClassName)
                .ToList();

            return groupedData;
        }
    }
}
