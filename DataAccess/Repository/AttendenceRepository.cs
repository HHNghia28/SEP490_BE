using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Context;
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
                .Include(a => a.AccountStudent)
                .ThenInclude(a => a.Student)
                .Where(a => Guid.Equals(a.ScheduleID, new Guid(slotID)))
                .Select(item => new AttendenceResponse()
                {
                    AttendenceID = item.ID.ToString(),
                    StudentID = item.StudentID,
                    StudentName = item.AccountStudent.Student.Fullname,
                    Avatar = item.AccountStudent.Student.Avatar,
                    Present = item.Present
                })
                .ToListAsync();

        }

        public async Task<IEnumerable<AttendenceResponse>> GetAttendenceStudent(string studentID, string subjectName, string schoolYear)
        {
            return await _context.Attendances
                .Include(a => a.AccountStudent)
                .ThenInclude(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Classes)
                .ThenInclude(a => a.SchoolYear)
                .Include(a => a.Schedule)
                .ThenInclude(a => a.Subject)
                .Where(a => a.AccountStudent.ID.ToLower().Equals(studentID.ToLower())
                && a.Schedule.Subject.Name.Equals(subjectName.ToLower())
                && a.Schedule.Classes.SchoolYear.Name.Equals(schoolYear.ToLower()))
                .Select(item => new AttendenceResponse()
                {
                    AttendenceID = item.ID.ToString(),
                    StudentID = item.StudentID,
                    StudentName = item.AccountStudent.Student.Fullname,
                    Avatar = item.AccountStudent.Student.Avatar,
                    Present = item.Present
                })
                .ToListAsync();
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
    }
}
