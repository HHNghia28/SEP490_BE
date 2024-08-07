using Azure.Core;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using ClosedXML.Excel;
using DataAccess.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public async Task AddClassesByExcel(string accountID, ExcelRequest request)
        {
            IFormFile file = request.File;
            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);

                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        foreach (var worksheet in workbook.Worksheets)
                        {
                            var classesRequest = new ClassesRequest();
                            var students = new List<string>();

                            classesRequest.TeacherID = worksheet.Cell("B1").GetString();
                            classesRequest.SchoolYear = worksheet.Cell("B2").GetString();
                            classesRequest.Classroom = worksheet.Cell("B3").GetString();

                            int row = 5;
                            while (!string.IsNullOrWhiteSpace(worksheet.Cell(row, 1).GetString()))
                            {
                                students.Add(worksheet.Cell(row, 1).GetString());
                                row++;
                            }

                            classesRequest.Students = students;

                            await AddClasses(accountID, classesRequest);
                        }
                    }
                }
            }
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

        public async Task<ClassResponse> GetClass(string className, string schoolYear)
        {
            Classes classes = await _context.Classes
                .Include(c => c.SchoolYear)
                .Include(c => c.Teacher)
                .Include(c => c.StudentClasses)
                .ThenInclude(c => c.AccountStudent)
                .ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(className.ToLower())
                                    && c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower())) ?? throw new NotFoundException("Lớp học không tồn tại");

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
                .OrderBy(c => c.SchoolYear.Name)
                .ThenBy(c => c.Classroom)
                .Select(item => new ClassesResponse()
                {
                    ID = item.ID,
                    Classroom = item.Classroom,
                    Teacher = item.Teacher.ID,
                    SchoolYear = item.SchoolYear.Name,
                })
                .OrderBy(c => c.Classroom)
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

        public async Task<byte[]> GenerateExcelFile(string className, string schoolYear)
        {
            Classes classes = await _context.Classes
                .Include(c => c.SchoolYear)
                .Include(c => c.Teacher)
                .Include(c => c.StudentClasses)
                .ThenInclude(c => c.AccountStudent)
                .ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(className.ToLower())
                                    && c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower())) ?? throw new NotFoundException("Lớp học không tồn tại");

            if (classes == null)
            {
                throw new NotFoundException("Không tìm thấy lớp");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Họ và tên";
                worksheet.Cell(1, 3).Value = "Giới tính";
                worksheet.Cell(1, 4).Value = "Địa chỉ";
                worksheet.Cell(1, 5).Value = "Tên đăng nhập";
                worksheet.Cell(1, 6).Value = "Tên đăng nhập phụ huynh";
                worksheet.Cell(1, 7).Value = "Mật khẩu (Lưu ý đây là mật khẩu mặc định vui lòng đổi mật khẩu khác)";

                int row = 2;
                foreach (var item in classes.StudentClasses)
                {
                    var student = item.AccountStudent.Student;
                    var accountStudent = item.AccountStudent;
                    worksheet.Cell(row, 1).Value = accountStudent.ID;
                    worksheet.Cell(row, 2).Value = student.Fullname;
                    worksheet.Cell(row, 3).Value = student.Gender;
                    worksheet.Cell(row, 4).Value = student.Address;
                    worksheet.Cell(row, 5).Value = accountStudent.Username;
                    worksheet.Cell(row, 6).Value = accountStudent.Username;
                    worksheet.Cell(row, 7).Value = "aA@123";
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    return stream.ToArray();
                }
            }
        }
    }
}
