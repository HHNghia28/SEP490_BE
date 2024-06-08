using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ClassesRepository : IClassesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;

        public ClassesRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
        }

        public async Task AddClasses(string accountID, ClassesRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Account teacher = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(request.TeacherID.ToLower())) ?? throw new ArgumentException("Tài khoản giáo viên chủ nhiệm không tồn tại");

            Guid schoolYearID = Guid.NewGuid();
            SchoolYear schoolYear = await _context.SchoolYears
                .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(request.SchoolYear.ToLower()));

            if (schoolYear == null)
            {
                await _context.SchoolYears.AddAsync(new SchoolYear()
                {
                    ID = schoolYearID,
                    Name = request.SchoolYear,
                });
            }
            else
            {
                schoolYearID = schoolYear.ID;
            }

            Classes classesExist = await _context.Classes
                .Include(c => c.SchoolYear)
                .FirstOrDefaultAsync(c => c.SchoolYear.Name.ToLower().Equals(request.SchoolYear.ToLower())
                && c.Classroom.ToLower().Equals(request.Classroom.ToLower()) && c.IsActive);

            if (classesExist != null)
            {
                throw new ArgumentException("Tên lớp đã tồn tại");
            }

            Guid classID = Guid.NewGuid();

            Classes classes = new()
            {
                ID = classID,
                Classroom = request.Classroom,
                TeacherID = request.TeacherID,
                SchoolYearID = schoolYearID,
                IsActive = true,
            };

            await _context.Classes.AddAsync(classes);

            List<StudentClasses> studentClasses = new();

            foreach (var item in request.Students)
            {
                StudentClasses studentClasses1 = await _context.StudentClasses
                    .Include(s => s.Classes)
                    .ThenInclude(s => s.SchoolYear)
                    .FirstOrDefaultAsync(s => s.StudentID.ToLower().Equals(item.ToLower())
                    && s.Classes.SchoolYear.Name.ToLower().Equals(request.SchoolYear.ToLower()));

                if (studentClasses1 != null) throw new ArgumentException("Học sinh " + item + " đã có lớp");

                studentClasses.Add(new StudentClasses()
                {
                    StudentID = item,
                    ClassID = classID,
                });
            }

            await _context.StudentClasses.AddRangeAsync(studentClasses);
            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện thêm lớp học " + request.Classroom,
                Type = LogName.CREATE.ToString(),
            });
        }

        public async Task DeleteClasses(string accountID, string classID)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Classes classes = await _context.Classes
                .Include(c => c.StudentClasses)
                .FirstOrDefaultAsync(c => c.ID.ToString().ToLower().Equals(classID.ToLower()));

            if (classes == null)
            {
                throw new NotFoundException("Không tìm thấy lớp");
            }

            classes.IsActive = false;

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện xóa lớp học " + classes.Classroom,
                Type = LogName.DELETE.ToString(),
            });
        }

        public async Task<ClassResponse> GetClass(string classID)
        {
            Classes classes = await _context.Classes
                .Include(c => c.SchoolYear)
                .Include(c => c.Teacher)
                .Include(c => c.StudentClasses)
                .ThenInclude(c => c.AccountStudent)
                .ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(c => c.ID.ToString().ToLower().Equals(classID.ToLower()));

            if (classes == null)
            {
                throw new NotFoundException("Không tìm thấy lớp");
            }

            List<ClassStudentResponse> students = new();

            foreach (var item in classes.StudentClasses)
            {
                students.Add(new ClassStudentResponse()
                {
                    ID = item.StudentID,
                    Fullname = item.AccountStudent.Student.Fullname,
                    Avatar = item.AccountStudent.Student.Avatar,
                    Gender = item.AccountStudent.Student.Gender
                });
            }

            return new ClassResponse()
            {
                ID = classes.ID,
                Classroom = classes.Classroom,
                SchoolYear = classes.SchoolYear.Name,
                Teacher = classes.Teacher.ID.ToString(),
                Students = students
            };
        }

        public async Task<IEnumerable<ClassesResponse>> GetClasses()
        {
            return await _context.Classes
                .Include(c => c.SchoolYear)
                .Include(c => c.Teacher)
                .Where(c => c.IsActive)
                .Select(item => new ClassesResponse()
                {
                    ID = item.ID,
                    Classroom = item.Classroom,
                    Teacher = item.Teacher.ID,
                    SchoolYear = item.SchoolYear.Name,
                })
                .ToListAsync();
        }

        public async Task UpdateClasses(string accountID, string classID, ClassesRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Classes classes = await _context.Classes
                .Include(c => c.StudentClasses)
                .FirstOrDefaultAsync(c => c.ID.ToString().ToLower().Equals(classID.ToLower()));

            if (classes == null)
            {
                throw new NotFoundException("Không tìm thấy lớp");
            }

            Guid schoolYearID = Guid.NewGuid();
            SchoolYear schoolYear = await _context.SchoolYears
                .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(request.SchoolYear.ToLower()));

            if (schoolYear == null)
            {
                await _context.SchoolYears.AddAsync(new SchoolYear()
                {
                    ID = schoolYearID,
                    Name = request.SchoolYear,
                });
            }
            else
            {
                schoolYearID = schoolYear.ID;
            }

            Classes classesName = await _context.Classes
                .Include(c => c.SchoolYear)
                .Where(c => !c.ID.ToString().ToLower().Equals(classID.ToLower()))
                .FirstOrDefaultAsync(c => c.SchoolYear.Name.ToLower().Equals(request.SchoolYear.ToLower())
                && c.Classroom.ToLower().Equals(request.Classroom.ToLower()) && c.IsActive);

            if (classesName != null)
            {
                throw new ArgumentException("Tên lớp đã tồn tại");
            }

            _context.StudentClasses.RemoveRange(classes.StudentClasses);

            classes.TeacherID = request.TeacherID;
            classes.SchoolYearID = schoolYearID;
            classes.Classroom = request.Classroom;

            List<StudentClasses> studentClasses = new();

            foreach (var item in request.Students)
            {
                studentClasses.Add(new StudentClasses()
                {
                    StudentID = item,
                    ClassID = new Guid(classID),
                });
            }

            await _context.StudentClasses.AddRangeAsync(studentClasses);
            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện chỉnh sửa lớp học " + classes.Classroom,
                Type = LogName.UPDATE.ToString(),
            });
        }
    }
}
