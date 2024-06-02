using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using ClosedXML.Excel;
using DataAccess.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;

        public ScoreRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
        }

        public async Task AddScoreByExcel(string accountID, ExcelRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(accountID.ToLower())) ?? throw new NotFoundException("Tài khoản của bạn không tồn tại");

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
                            List<string> data = new();

                            for (int row = 1; row <= worksheet.LastRowUsed().RowNumber(); row++)
                            {
                                for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                                {
                                    var cell = worksheet.Cell(row, col);
                                    data.Add(cell.Value.ToString());
                                }
                            }

                            string str = "";
                            string strClass = "";
                            string strSchoolYear = "";
                            string strSemester = "";
                            string strSubject = "";
                            string strScore = "";
                            int indexCol = 0;
                            Dictionary<string, string> strScores = new Dictionary<string, string>();

                            for (int i = 0; i < data.Count; i++)
                            {
                                str = data.ElementAt(i);

                                switch (str)
                                {
                                    case "Lớp":
                                        i++;
                                        strClass = data.ElementAt(i);
                                        break;
                                    case "Năm học":
                                        i++;
                                        strSchoolYear = data.ElementAt(i);
                                        break;
                                    case "Học kỳ":
                                        i++;
                                        strSemester = data.ElementAt(i);
                                        break;
                                    case "Môn học":
                                        i++;
                                        strSubject = data.ElementAt(i);
                                        break;
                                    case "Cột điểm":
                                        i++;
                                        strScore = data.ElementAt(i);
                                        break;
                                    case "Lần thứ":
                                        i++;
                                        indexCol = int.Parse(data.ElementAt(i).ToString());
                                        break;
                                    case "Danh sách":
                                        i++;
                                        for (int j = i; j < data.Count - 1; j++)
                                        {
                                            i++;
                                            str = data.ElementAt(i);

                                            if(!string.IsNullOrEmpty(str))
                                            {
                                                i++;
                                                j++;
                                                int s = int.Parse(data.ElementAt(i));
                                                if (s < 0 || s > 10) throw new ArgumentException("Điểm phải nằm trong thang điểm 10");
                                                strScores.Add(str.ToLower(), data.ElementAt(i));
                                            }
                                        }
                                        break;
                                }
                            }

                            Classes classes = await _context.Classes
                                .Include(c => c.SchoolYear)
                                .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(strClass.ToLower())
                                    && c.SchoolYear.Name.ToLower().Equals(strSchoolYear.ToLower())) ?? throw new NotFoundException("Lớp học không tồn tại");

                            List<AccountStudent> students = await _context.AccountStudents
                                .Where(a => strScores.Keys.Contains(a.ID.ToLower()))
                                .ToListAsync();

                            if (students.Count <= 0) throw new NotFoundException("Không tìm thấy học sinh nào");

                            Subject subject = await _context.Subjects
                                .FirstOrDefaultAsync(s => s.IsActive && s.Name.ToLower().Equals(strSubject.ToLower())
                                && s.Grade.Equals(strClass.Substring(0, 2))) ?? throw new NotFoundException("Môn học không tồn tại");

                            ComponentScore componentScore = await _context.ComponentScores
                                .Include(c => c.Subject)
                                .FirstOrDefaultAsync(c => c.Name.ToLower().Equals(strScore.ToLower())
                                && c.Semester.ToLower().Equals(strSemester.ToLower()) 
                                && Guid.Equals(subject.ID, c.Subject.ID)) ?? throw new NotFoundException("Điểm thành phần không tồn tại");

                            StudentScores studentScores1 = await _context.StudentScores
                                .FirstOrDefaultAsync(s => s.StudentID.ToLower().Equals(students.ElementAt(0).ID.ToLower())
                                && s.Name.ToLower().Equals(componentScore.Name.ToLower())
                                && s.Semester.ToLower().Equals(componentScore.Semester.ToLower())
                                && s.IndexColumn == indexCol);

                            if (studentScores1 != null)
                            {
                                throw new ArgumentException("Cột điểm " + studentScores1.Name + " lần thứ " + studentScores1.IndexColumn + " đã tồn tại");
                            }

                            List<StudentScores> check = await _context.StudentScores
                                .Where(s => s.StudentID.ToLower().Equals(students.ElementAt(0).ID.ToLower())
                                && s.Name.ToLower().Equals(componentScore.Name.ToLower())
                                && s.Semester.ToLower().Equals(componentScore.Semester.ToLower()))
                                .ToListAsync();

                            if (check.Count >= componentScore.Count) throw new ArgumentException("Cột điểm đã đạt giới hạn tối đa " + componentScore.Count);

                            List<StudentScores> studentScores = students.Select(item => new StudentScores()
                            {
                                ID = Guid.NewGuid(),
                                Name = componentScore.Name,
                                SchoolYearID = classes.SchoolYearID,
                                Score = strScores[item.ID.ToLower()],
                                ScoreFactor = componentScore.ScoreFactor,
                                Semester = strSemester,
                                StudentID = item.ID,
                                IndexColumn = indexCol,
                                Subject = subject.Name
                            })
                            .ToList();

                            await _context.StudentScores.AddRangeAsync(studentScores);
                            await _context.SaveChangesAsync();

                            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
                            {
                                AccountID = accountID,
                                Note = "Người dùng " + account.Username + " vừa thực hiện nhập điểm " + strScore.ToLower() + " của lớp " + classes.Classroom,
                                Type = LogName.CREATE.ToString(),
                            });
                        }
                    }
                }
            }
        }

        public async Task DeleteScore()
        {
            _context.StudentScores.RemoveRange(await _context.StudentScores.ToListAsync());
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> GenerateExcelFile(string className, string schoolYear, string semester, string subjectName, string component)
        {
            Classes classes = await _context.Classes
                                .Include(c => c.SchoolYear)
                                .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(className.ToLower())
                                    && c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower())) ?? throw new NotFoundException("Lớp học không tồn tại");

            Subject subject = await _context.Subjects
                                .Include(s => s.ComponentScores)
                                .FirstOrDefaultAsync(s => s.IsActive && s.Name.ToLower().Equals(subjectName.ToLower())
                                && s.Grade.Equals(className.Substring(0, 2))) ?? throw new NotFoundException("Môn học không tồn tại");

            ComponentScore componentScore = await _context.ComponentScores
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Name.ToLower().Equals(component.ToLower())
                && c.Semester.ToLower().Equals(semester.ToLower())
                && Guid.Equals(subject.ID, c.Subject.ID)) ?? throw new NotFoundException("Điểm thành phần không tồn tại");

            List<StudentClasses> students = await _context.StudentClasses
                .Where(s => Guid.Equals(s.ClassID, classes.ID)).ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(classes.Classroom);

                // Tạo tiêu đề cột
                worksheet.Cell(1, 1).Value = "Lớp";
                worksheet.Cell(2, 1).Value = "Năm học";
                worksheet.Cell(3, 1).Value = "Học kỳ";
                worksheet.Cell(4, 1).Value = "Môn học";
                worksheet.Cell(5, 1).Value = "Cột điểm";
                worksheet.Cell(6, 1).Value = "Lần thứ";
                worksheet.Cell(1, 2).Value = classes.Classroom;
                worksheet.Cell(2, 2).Value = schoolYear;
                worksheet.Cell(3, 2).Value = semester;
                worksheet.Cell(4, 2).Value = subject.Name;
                worksheet.Cell(5, 2).Value = componentScore.Name;
                worksheet.Cell(6, 2).Value = 1;

                worksheet.Cell(8, 1).Value = "Danh sách";

                for (int i = 0; i < students.Count; i++)
                {
                    worksheet.Cell(9 + i, 1).Value = students.ElementAt(i).StudentID;
                    worksheet.Cell(9 + i, 2).Value = 0;
                }

                // Lưu file Excel vào MemoryStream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
