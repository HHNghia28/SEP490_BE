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
        private readonly IActivityLogRepository _activityLogRepository;

        public RegisterBookRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<RegistersBookResponse> GetRegistersBook(string classID, string fromDate)
        {
            List<DateTime> dates = GetDatesToNextSunday(DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

            var schedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                    .ThenInclude(s => s.SchoolYear)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                    .ThenInclude(s => s.LessonPlans)
                .Include(s => s.Attendances)
                    .ThenInclude(a => a.AccountStudent)
                    .ThenInclude(a => a.Student)
                .Where(s => s.ClassID == new Guid(classID) && dates.Contains(s.Date))
                .Select(s => new
                {
                    s.ID,
                    s.Date,
                    s.SubjectID,
                    s.SlotByDate,
                    s.Subject,
                    s.Teacher,
                    s.SlotByLessonPlans,
                    s.Note,
                    s.Rank,
                    s.Classes,
                    Attendances = s.Attendances.Where(a => dates.Contains(a.Date)).ToList()
                })
                .OrderBy(s => s.Date)
                .ThenBy(s => s.SlotByDate)
                .ToListAsync();

            if (!schedules.Any())
            {
                throw new ArgumentException("Không tìm thấy sổ đầu bài");
            }

            var classInfo = schedules.First().Classes;
            var response = new RegistersBookResponse
            {
                Classname = classInfo.Classroom,
                FromDate = dates.First().ToString("dd/MM/yyyy"),
                ToDate = dates.Last().ToString("dd/MM/yyyy"),
                SchoolYear = classInfo.SchoolYear.Name,
            };

            var lessonPlanDict = schedules
                .SelectMany(s => s.Subject.LessonPlans)
                .GroupBy(lp => new { lp.SubjectID, lp.Slot })
                .ToDictionary(g => g.Key, g => g.First().Title);

            var responseDetails = dates.Select(date =>
            {
                var slots = schedules
                    .Where(s => s.Date == date)
                    .Select(s => new RegistersBookSlotResponse
                    {
                        ID = s.ID.ToString(),
                        Slot = s.SlotByDate,
                        Subject = s.Subject.Name,
                        Teacher = s.Teacher.Username,
                        SlotByLessonPlan = s.SlotByLessonPlans,
                        NumberOfAbsent = s.Attendances.Count(a => !a.Present),
                        NumberAbsent = s.Attendances.Where(a => !a.Present).Select(a => a.AccountStudent.Student.Fullname).ToList(),
                        Note = s.Note,
                        Rating = s.Rank,
                        Title = lessonPlanDict.TryGetValue(new { s.SubjectID, Slot = s.SlotByLessonPlans }, out var title) ? title : string.Empty
                    })
                    .ToList();

                return new RegistersBookDetailResponse
                {
                    ID = Guid.NewGuid().ToString(),
                    Date = date.ToString("dd/MM/yyyy"),
                    WeekDate = GetVietnameseDayOfWeek(date.DayOfWeek),
                    Slots = slots
                };
            }).ToList();

            response.Details = responseDetails;

            return response;
        }

        public async Task UpdateRegisterBook(string accountID, RegisterBookUpdateRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Schedule schedule = await _context.Schedules
                .Include(s => s.Classes)
                .FirstOrDefaultAsync(s => Guid.Equals(s.ID, new Guid(request.ID))) ?? throw new ArgumentException("Tiết học không tồn tại");

            schedule.Note = request.Note;
            schedule.Rank = request.Rating;

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện cập nhật sổ đầu bài tiết " + schedule.SlotByDate 
                + " ngày " + schedule.Date.ToString("dd/MM/yyyy") + " lớp " + schedule.Classes.Classroom,
                Type = LogName.UPDATE.ToString(),
            });
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
