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

        public ClassesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddClasses(ClassesRequest request)
        {
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
                studentClasses.Add(new StudentClasses()
                {
                    StudentID = item,
                    ClassID = classID,
                });
            }

            await _context.StudentClasses.AddRangeAsync(studentClasses);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClasses(string classID)
        {
            Classes classes = await _context.Classes
                .Include(c => c.StudentClasses)
                .FirstOrDefaultAsync(c => c.ID.ToString().ToLower().Equals(classID.ToLower()));

            if (classes == null)
            {
                throw new NotFoundException("Không tìm thấy lớp");
            }

            classes.IsActive = false;

            await _context.SaveChangesAsync();
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

        public async Task UpdateClasses(string classID, ClassesRequest request)
        {
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
        }
    }
}
