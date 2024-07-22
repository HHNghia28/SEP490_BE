using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Context;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class AttendenceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;

        public AttendenceRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<IEnumerable<AttendenceResponse>> GetAttendenceBySlot(string slotID)
        {
            return await _context.Attendances
                .AsNoTracking()
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Subject)
                .Include(a => a.AccountStudent)
                .ThenInclude(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Teacher)
                .Where(a => Guid.Equals(a.ScheduleID, new Guid(slotID)))
                .OrderBy(item => item.Schedule.Date)
                .ThenBy(item => item.Schedule.SlotByLessonPlans)
                .Select(item => new AttendenceResponse()
                {
                    AttendenceID = item.ID.ToString(),
                    StudentID = item.StudentID,
                    StudentName = item.AccountStudent.Student.Fullname,
                    Avatar = item.AccountStudent.Student.Avatar,
                    Present = item.Present,
                    Confirmed = item.Confirmed,
                    Date = item.Schedule.Date.ToString("dd/MM/yyyy"),
                    Subject = item.Schedule.Subject.Name,
                    Status = item.Schedule.Date > DateTime.Now ? "Chưa bắt đầu" : item.Present ? "Có mặt" : item.Confirmed ? "Vắng có phép" : "Vắng không phép",
                    Teacher = item.Schedule.Teacher.Username,
                    Slot = item.Schedule.SlotByLessonPlans
                })
                .ToListAsync();

        }

        public async Task<IEnumerable<AttendenceResponse>> GetAttendenceStudent(string studentID, string subjectName, string schoolYear)
        {
            return await _context.Attendances
                .AsNoTracking()
                .Include(a => a.AccountStudent)
                .ThenInclude(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Classes)
                .ThenInclude(a => a.SchoolYear)
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Subject)
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Teacher)
                .Where(a => a.AccountStudent.ID.ToLower().Equals(studentID.ToLower())
                && a.Schedule.Subject.Name.Equals(subjectName.ToLower())
                && a.Schedule.Classes.SchoolYear.Name.Equals(schoolYear.ToLower()))
                .OrderBy(item => item.Schedule.Date)
                .ThenBy(item => item.Schedule.SlotByLessonPlans)
                .Select(item => new AttendenceResponse()
                {
                    AttendenceID = item.ID.ToString(),
                    StudentID = item.StudentID,
                    StudentName = item.AccountStudent.Student.Fullname,
                    Avatar = item.AccountStudent.Student.Avatar,
                    Present = item.Present,
                    Confirmed = item.Confirmed,
                    Date = item.Schedule.Date.ToString("dd/MM/yyyy"),
                    Subject = item.Schedule.Subject.Name,
                    Status = item.Schedule.Date > DateTime.Now ? "Chưa bắt đầu" : item.Present ? "Có mặt" : item.Confirmed ? "Vắng có phép" : "Vắng không phép",
                    Teacher = item.Schedule.Teacher.Username,
                    Slot = item.Schedule.SlotByLessonPlans
                })
                .ToListAsync();
        }

        public async Task<Dictionary<string, Dictionary<string, object>>> GetAttendenceStudentAllSubject(string studentID, string schoolYear)
        {
            AccountStudent student = await _context.AccountStudents
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(studentID.ToLower())
                && a.IsActive) ?? throw new NotFoundException("Học sinh không tồn tại");

            Classes classes = await _context.Classes
                .Include(c => c.StudentClasses)
                .ThenInclude(c => c.AccountStudent)
                .ThenInclude(c => c.Student)
                .Include(c => c.SchoolYear)
                .Include(c => c.Teacher)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(c => c.StudentClasses.Select(c => c.StudentID.ToLower()).Contains(studentID.ToLower())
                    && c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower())) ?? throw new NotFoundException("Lớp học không tồn tại");

            List<Subject> subjects = await _context.Subjects
                .Include(s => s.ComponentScores)
                .Include(s => s.Schedules)
                .Where(s => s.Schedules.Select(s => s.ClassID.ToString().ToLower()).Contains(classes.ID.ToString().ToLower())
                    && s.ComponentScores.Count > 0)
                .ToListAsync();

            Dictionary<string, Dictionary<string, object>> response = new();

            foreach (var subject in subjects)
            {
                var attendances = await _context.Attendances
                    .AsNoTracking()
                    .Include(a => a.AccountStudent)
                    .ThenInclude(a => a.Student)
                    .Include(a => a.Schedule)
                    .ThenInclude(a => a.Classes)
                    .ThenInclude(a => a.SchoolYear)
                    .Include(a => a.Schedule)
                    .ThenInclude(a => a.Subject)
                    .Where(a => a.AccountStudent.ID.ToLower().Equals(studentID.ToLower())
                        && a.Schedule.Subject.Name.ToLower().Equals(subject.Name.ToLower())
                        && a.Schedule.Classes.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower()))
                    .OrderBy(a => a.Schedule.Date)
                    .ToListAsync();

                int daysPresent = 0;
                int daysComfirmed = 0;
                int daysAbsent = 0;
                int daysNotStarted = 0;
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (attendances.Any())
                {
                    startDate = attendances.First().Schedule.Date;
                    endDate = attendances.Last().Schedule.Date;
                }

                foreach (var attendance in attendances)
                {
                    if (attendance.Schedule.Date > DateTime.Now)
                    {
                        daysNotStarted++;
                    }
                    else if (attendance.Present)
                    {
                        daysPresent++;
                    }
                    else if (attendance.Confirmed)
                    {
                        daysComfirmed++;
                    }
                    else
                    {
                        daysAbsent++;
                    }
                }

                response.Add(subject.Name, new Dictionary<string, object>
                {
                    { "Có mặt", daysPresent },
                    { "Vắng không phép", daysAbsent },
                    { "Vắng có phép", daysComfirmed },
                    { "Chưa bắt đầu", daysNotStarted },
                    { "Ngày bắt đầu", startDate?.ToString("dd/MM/yyyy") },
                    { "Ngày kết thúc", endDate?.ToString("dd/MM/yyyy") }
                });
            }

            return response;
        }

        public async Task UpdateAttendence(string accountID, List<AttendenceRequest> requests)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            List<Attendance> attendances = await _context.Attendances
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Classes)
                .Where(a => requests.Select(item => new Guid(item.AttendenceID)).Contains(a.ID))
                .ToListAsync();

            if (attendances.Count <= 0)
            {
                throw new NotFoundException("Không tìm thấy lớp học");
            }

            foreach (var item in attendances)
            {
                AttendenceRequest request = requests.FirstOrDefault(r => Guid.Equals(item.ID, new Guid(r.AttendenceID)));

                if (request == null) continue;

                item.Present = request.Present;
                item.Confirmed = request.Confirmed;
            }

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện điểm danh tiết " + attendances.ElementAt(0).Schedule.SlotByDate
                + " ngày " + attendances.ElementAt(0).Date.ToString("dd/MM/yyyy") + " lớp học " + attendances.ElementAt(0).Schedule.Classes.Classroom,
                Type = LogName.CREATE.ToString(),
            });
        }

        public async Task<IEnumerable<AttendanceTeacherResponse>> GetAttendanceTeacherResponses(string teacherID, string schoolYear)
        {
            var teacherAccount = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(teacherID.ToLower()))
                ?? throw new ArgumentException("Teacher account does not exist");

            var schedules = await _context.Schedules
                .Include(s => s.Subject)
                .Include(s => s.Classes)
                .ThenInclude( s=> s.SchoolYear)
                .Where(s => s.Classes.SchoolYear.Name == schoolYear && s.TeacherID.ToLower().Equals(teacherAccount.ID.ToLower()))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.SlotByDate)
                .Select(s => new AttendanceTeacherResponse
                {
                    ScheduleID = s.ID.ToString(),
                    TeacherID = teacherAccount.ID,
                    TeacherName = teacherAccount.User.Fullname,
                    Avatar = teacherAccount.User.Avatar,
                    Classname = s.Classes.Classroom,
                    Present = !s.Rank.Equals(""),
                    Date = s.Date.ToString("dd/MM/yyyy"),
                    Subject = s.Subject.Name,
                    Status = !s.Rank.Equals("") ? "Có mặt" : "Vắng", 
                    Slot = s.SlotByDate
                })
                .ToListAsync();

            return schedules;
        }
    }
}
