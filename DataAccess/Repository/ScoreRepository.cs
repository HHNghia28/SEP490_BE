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
                                Note = "Người dùng " + account.Username + " vừa thực hiện nhập điểm " + strScore.ToLower() + " lần thứ " + indexCol + " của lớp " + classes.Classroom,
                                Type = LogName.CREATE.ToString(),
                            });
                        }
                    }
                }
            }
        }

        public async Task<byte[]> GenerateExcelFile(string className, string schoolYear, string semester, string subjectName, string component, int indexCol = 1)
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
                .Include(s => s.AccountStudent)
                .ThenInclude(s => s.Scores)
                .Where(s => Guid.Equals(s.ClassID, classes.ID)).ToListAsync();

            List<StudentScores> scores = await _context.StudentScores
                .Where(s => Guid.Equals(s.SchoolYearID, classes.SchoolYearID)
                && s.Subject.ToLower().Equals(subject.Name.ToLower())
                && s.Name.ToLower().Equals(componentScore.Name.ToLower())
                && s.IndexColumn == indexCol)
                .ToListAsync();

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
                    StudentScores score = scores.FirstOrDefault(s => s.StudentID.ToLower().Equals(students.ElementAt(i).StudentID.ToLower()));

                    worksheet.Cell(9 + i, 1).Value = students.ElementAt(i).StudentID;
                    worksheet.Cell(9 + i, 2).Value = scores!= null && score != null ? int.Parse(score.Score) : 0;
                }

                // Lưu file Excel vào MemoryStream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<ScoresResponse> GetScoresByClassBySubject(string className, string subjectName, string schoolYear)
        {
            Classes classes = await _context.Classes
                                .Include(c => c.StudentClasses)
                                .ThenInclude(c => c.AccountStudent)
                                .ThenInclude(c => c.Student)
                                .Include(c => c.SchoolYear)
                                .Include(c => c.Teacher)
                                .ThenInclude(c => c.User)
                                .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(className.ToLower())
                                    && c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower())) ?? throw new NotFoundException("Lớp học không tồn tại");

            Subject subject = await _context.Subjects
                                .Include(s => s.ComponentScores)
                                .FirstOrDefaultAsync(s => s.IsActive && s.Name.ToLower().Equals(subjectName.ToLower())
                                && s.Grade.Equals(className.Substring(0, 2))) ?? throw new NotFoundException("Môn học không tồn tại");

            List<StudentScores> studentScores = await _context.StudentScores
                .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                && s.Subject.ToLower().Equals(subject.Name.ToLower()))
                .ToListAsync();

            List<ScoreResponse> scores = new List<ScoreResponse>();

            Dictionary<double, int> ranks = new();

            foreach (var item in classes.StudentClasses)
            {
                List<StudentScores> details = studentScores
                    .Where(s => s.StudentID.ToLower().Equals(item.StudentID.ToLower()))
                    .ToList();

                List<ScoreDetailResponse> scoreDetails = details
                    .Select(item1 => new ScoreDetailResponse()
                    {
                        Key = item1.Name,
                        Semester = item1.Semester,
                        Value = double.Parse(item1.Score)
                    })
                    .ToList();

                double sum = 0;
                decimal count = 0;

                foreach (var item1 in details)
                {
                    if (double.TryParse(item1.Score, out double score))
                    {
                        sum += score * (double)item1.ScoreFactor;
                        count += item1.ScoreFactor;
                    }
                    else
                    {
                        // Handle the case where the score is not a valid double
                        // For example, log the error or set a default value
                    }
                }

                double average = double.IsNaN((double)Math.Round(sum / (double)count)) ? 0 : (double)Math.Round(sum / (double)count);

                if (!ranks.ContainsKey(average))
                {
                    ranks.Add(average, 0);
                }

                scores.Add(new ScoreResponse()
                {
                    ID = item.StudentID,
                    FullName = item.AccountStudent.Student.Fullname,
                    Average = double.IsNaN((double)Math.Round(sum / (double)count)) ? 0 : (double)Math.Round(sum / (double)count),
                    Scores = scoreDetails
                });
            }

            Dictionary<double, int> uniqueDict = new();
            foreach (var kvp in ranks)
            {
                if (!uniqueDict.ContainsKey(kvp.Key))
                {
                    uniqueDict[kvp.Key] = kvp.Value;
                }
            }

            // Step 2: Sort the dictionary by keys in descending order
            var sortedDict = uniqueDict
                .OrderByDescending(kvp => kvp.Key)
                .ToList();

            // Step 3: Assign new values based on the order
            Dictionary<double, int> resultDict = new Dictionary<double, int>();
            for (int i = 0; i < sortedDict.Count; i++)
            {
                resultDict[sortedDict[i].Key] = i + 1;  // Value starts from 1 and increments
            }

            foreach (var item in scores)
            {
                item.Rank = resultDict[item.Average];
            }

            return new ScoresResponse()
            {
                Class = classes.Classroom,
                SchoolYear = schoolYear,
                Subject =subject.Name,
                TeacherName = classes.Teacher.User.Fullname,
                Score = scores
            };
        }

        public async Task<ScoreStudentResponse> GetScoresByStudentAllSubject(string studentID, string schoolYear)
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

            List<ScoreSubjectResponse> scores = new();

            foreach (var item in subjects)
            {
                List<StudentScores> studentScores = await _context.StudentScores
                .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                && s.Subject.ToLower().Equals(item.Name.ToLower())
                && s.StudentID.ToLower().Equals(student.ID.ToLower()))
                .ToListAsync();

                List<ScoreDetailResponse> scoreDetails = studentScores
                    .Select(item1 => new ScoreDetailResponse()
                    {
                        Key = item1.Name,
                        Semester = item1.Semester,
                        Value = double.Parse(item1.Score)
                    })
                    .ToList();

                double sum = 0;
                decimal count = 0;

                foreach (var item1 in studentScores)
                {
                    if (double.TryParse(item1.Score, out double score))
                    {
                        sum += score * (double)item1.ScoreFactor;
                        count += item1.ScoreFactor;
                    }
                    else
                    {
                        // Handle the case where the score is not a valid double
                        // For example, log the error or set a default value
                    }
                }

                scores.Add(new ScoreSubjectResponse()
                {
                    Subject = item.Name,
                    Average = double.IsNaN((double)Math.Round(sum / (double)count)) ? 0 : (double)Math.Round(sum / (double)count),
                    Scores = scoreDetails,
                });
            }

            ScoreStudentResponse response = new ScoreStudentResponse()
            {
                ClassName = classes.Classroom,
                FullName = student.Student.Fullname,
                SchoolYear = schoolYear,
                Details = scores
            };

            return response;
        }

        public async Task<ScoreStudentResponse> GetScoresByStudentBySubject(string studentID, string subject, string schoolYear)
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
                && s.ComponentScores.Count > 0
                && s.Name.ToLower().Equals(subject.ToLower()))
                .ToListAsync();

            List<ScoreSubjectResponse> scores = new();

            foreach (var item in subjects)
            {
                List<StudentScores> studentScores = await _context.StudentScores
                .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                && s.Subject.ToLower().Equals(item.Name.ToLower())
                && s.StudentID.ToLower().Equals(student.ID.ToLower()))
                .ToListAsync();

                List<ScoreDetailResponse> scoreDetails = studentScores
                    .Select(item1 => new ScoreDetailResponse()
                    {
                        Key = item1.Name,
                        Semester = item1.Semester,
                        Value = double.Parse(item1.Score)
                    })
                    .ToList();

                double sum = 0;
                decimal count = 0;

                foreach (var item1 in studentScores)
                {
                    if (double.TryParse(item1.Score, out double score))
                    {
                        sum += score * (double)item1.ScoreFactor;
                        count += item1.ScoreFactor;
                    }
                    else
                    {
                        // Handle the case where the score is not a valid double
                        // For example, log the error or set a default value
                    }
                }

                scores.Add(new ScoreSubjectResponse()
                {
                    Subject = item.Name,
                    Average = double.IsNaN((double)Math.Round(sum / (double)count)) ? 0 : (double)Math.Round(sum / (double)count),
                    Scores = scoreDetails,
                });
            }

            ScoreStudentResponse response = new ScoreStudentResponse()
            {
                ClassName = classes.Classroom,
                FullName = student.Student.Fullname,
                SchoolYear = schoolYear,
                Details = scores
            };

            return response;
        }

        public async Task UpdateScoreByExcel(string accountID, ExcelRequest request)
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

                                            if (!string.IsNullOrEmpty(str))
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

                            List<StudentScores> scores = await _context.StudentScores
                                .Where(s => Guid.Equals(s.SchoolYearID, classes.SchoolYearID)
                                && s.Subject.ToLower().Equals(subject.Name.ToLower())
                                && s.Name.ToLower().Equals(componentScore.Name.ToLower())
                                && s.IndexColumn == indexCol)
                                .ToListAsync();

                            if (scores.Count <= 0) throw new NotFoundException("Điểm thành phần không tồn tại");

                            foreach (var item in scores)
                            {
                                string score = strScores[item.StudentID.ToString().ToLower()] ?? item.Score;
                                item.Score = score;
                            }

                            await _context.SaveChangesAsync();

                            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
                            {
                                AccountID = accountID,
                                Note = "Người dùng " + account.Username + " vừa thực hiện cập nhật điểm " + strScore.ToLower() + " lần thứ " + indexCol + " của lớp " + classes.Classroom,
                                Type = LogName.UPDATE.ToString(),
                            });
                        }
                    }
                }
            }
        }
    }
}
