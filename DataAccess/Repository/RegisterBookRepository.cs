using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Context;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RegisterBookRepository : IRegisterBookRepository
    {
        private readonly ApplicationDbContext _context;

        public RegisterBookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegistersBookResponse> GetRegistersBook(string classID, string fromDate)
        {
            List<DateTime> dates = GetDatesToNextSunday(DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                .ThenInclude(s => s.SchoolYear)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .ThenInclude(s => s.LessonPlans)
                .Include(s => s.Attendances)
                .ThenInclude(s => s.AccountStudent)
                .ThenInclude(s => s.Student)
                .Where(s => Guid.Equals(s.ClassID, new Guid(classID)) && dates.Contains(s.Date))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.SlotByDate)
                .ToListAsync();

            if (schedules.Count == 0) throw new ArgumentException("Không tìm thấy sổ đầu bài"); 

            RegistersBookResponse response = new()
            {
                Classname = schedules.ElementAt(0).Classes.Classroom,
                FromDate = dates.ElementAt(0).ToString("dd/MM/yyyy"),
                ToDate = dates.ElementAt(dates.Count - 1).ToString("dd/MM/yyyy"),
                SchoolYear = schedules.ElementAt(0).Classes.SchoolYear.Name,
            };

            List<RegistersBookDetailResponse> responseDetails = new();

            foreach (var date in dates)
            {
                List<RegistersBookSlotResponse> registersBookSlotResponses = schedules
                    .Where(s => s.Date == date)
                    .Select(item => new RegistersBookSlotResponse()
                    {
                        ID = item.ID.ToString(),
                        Slot = item.SlotByDate,
                        Subject = item.Subject.Name,
                        Teacher = item.Teacher.Username,
                        SlotByLessonPlan = item.SlotByLessonPlans,
                        NumberOfAbsent = item.Attendances.Where(s => !s.Present).Count(),
                        NumberAbsent = item.Attendances.Where(s => !s.Present).Select(s => s.AccountStudent.Student.Fullname).ToList(),
                        Note = item.Note,
                        Rating = item.Rank,
                        Title = item.Subject.LessonPlans.FirstOrDefault(l => l.Slot == item.SlotByLessonPlans) != null ? item.Subject.LessonPlans.FirstOrDefault(l => l.Slot == item.SlotByLessonPlans).Title : ""
                    })
                    .ToList();

                responseDetails.Add(new RegistersBookDetailResponse()
                {
                    ID = Guid.NewGuid().ToString(),
                    Date = date.ToString("dd/MM/yyyy"),
                    WeekDate = GetVietnameseDayOfWeek(date.DayOfWeek),
                    Slots = registersBookSlotResponses
                });
            }

            response.Details = responseDetails;

            return response;
        }

        private List<DateTime> GetDatesToNextSunday(DateTime startDate)
        {
            List<DateTime> dates = new List<DateTime>();

            DateTime currentDate = startDate;

            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                currentDate = currentDate.AddDays(1);
            }

            while (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                dates.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            dates.Add(currentDate);

            return dates;
        }

        private string GetVietnameseDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Thứ Hai";
                case DayOfWeek.Tuesday:
                    return "Thứ Ba";
                case DayOfWeek.Wednesday:
                    return "Thứ Tư";
                case DayOfWeek.Thursday:
                    return "Thứ Năm";
                case DayOfWeek.Friday:
                    return "Thứ Sáu";
                case DayOfWeek.Saturday:
                    return "Thứ Bảy";
                case DayOfWeek.Sunday:
                    return "Chủ Nhật";
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Ngày không hợp lệ");
            }
        }

        private string GetSlotTime(int i)
        {
            switch (i)
            {
                case 1:
                    return "7h10-7h55";
                case 2:
                    return "8h00-8h45";
                case 3:
                    return "9h05-9h50";
                case 4:
                    return "9h55-10h40";
                case 5:
                    return "10h50-11h35";
                case 6:
                    return "12h45-13h20";
                case 7:
                    return "13h25-14h10";
                case 8:
                    return "14h30-15h15";
                case 9:
                    return "15h20-16h05";
                case 10:
                    return "16h15-17h00";
            }

            return "";
        }
    }
}
