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

                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                i++;
                                                j++;
                                                string score = data.ElementAt(i);

                                                // Convert the score to lowercase for comparison
                                                string lowerScore = score.ToLower();

                                                // Check if the score is a valid integer or one of the special cases
                                                if (int.TryParse(score, out int s))
                                                {
                                                    if (s < 0 || s > 10)
                                                    {
                                                        throw new ArgumentException("Điểm phải nằm trong thang điểm 10");
                                                    }
                                                }
                                                else if (lowerScore != "đ" && lowerScore != "cđ")
                                                {
                                                    throw new ArgumentException("Điểm phải nằm trong thang điểm 10 hoặc là Đ, đ, CĐ, cđ");
                                                }

                                                // Add the score to the dictionary
                                                strScores.Add(str.ToLower(), score);
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
                worksheet.Cell(6, 2).Value = indexCol;

                worksheet.Cell(8, 1).Value = "Danh sách";

                for (int i = 0; i < students.Count; i++)
                {
                    StudentScores score = scores.FirstOrDefault(s => s.StudentID.ToLower().Equals(students.ElementAt(i).StudentID.ToLower()));

                    worksheet.Cell(9 + i, 1).Value = students.ElementAt(i).StudentID;
                    worksheet.Cell(9 + i, 2).Value = scores != null && score != null ? double.Parse(score.Score) : 0;
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
                .Include(s => s.SchoolYear)
                .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                && s.Subject.ToLower().Equals(subject.Name.ToLower())
                && s.SchoolYear.Name.ToLower().Equals(classes.SchoolYear.Name.ToLower()))
                .ToListAsync();

            List<ScoreResponse> scores = new List<ScoreResponse>();

            Dictionary<double, int> ranks = new();

            foreach (var studentClass in classes.StudentClasses)
            {
                var studentScoresBySubject = studentScores
                    .Where(s => s.StudentID.ToLower().Equals(studentClass.StudentID.ToLower()))
                    .ToList();

                var scoreDetails = studentScoresBySubject
                    .Select(s => new ScoreDetailResponse
                    {
                        Key = s.Name,
                        Semester = s.Semester,
                        Value = s.Score,
                        IndexCol = s.IndexColumn
                    })
                    .OrderBy(s => s.Semester)
                    .ThenBy(s => s.Key)
                    .ThenBy(s => s.IndexCol)
                    .ToList();

                double sumSemester1 = 0, sumSemester2 = 0, totalSum = 0;
                decimal countSemester1 = 0, countSemester2 = 0, totalCount = 0;

                bool allPassSemester1 = true, allPassSemester2 = true, allPassYear = true;
                bool hasNegativeOneScore_Sem1 = false, hasNegativeOneScore_Sem2 = false, hasNegativeOneScore_Year = false;

                foreach (var score in studentScoresBySubject)
                {
                    if (score.Score.Equals("Đ", StringComparison.OrdinalIgnoreCase) || score.Score.Equals("CĐ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (score.Semester == "Học kỳ I")
                        {
                            if (score.Score != "Đ")
                            {
                                allPassSemester1 = false;
                            }
                        }
                        else if (score.Semester == "Học kỳ II")
                        {
                            if (score.Score != "Đ")
                            {
                                allPassSemester2 = false;
                            }
                        }

                        if (score.Score != "Đ")
                        {
                            allPassYear = false;
                        }
                    }
                    else if (double.TryParse(score.Score, out double scoreValue))
                    {
                        if (scoreValue == -1)
                        {
                            hasNegativeOneScore_Year = true;
                            if (score.Semester == "Học kỳ I")
                            {
                                hasNegativeOneScore_Sem1 = true;
                            }
                            else if (score.Semester == "Học kỳ II")
                            {
                                hasNegativeOneScore_Sem2 = true;
                            }
                        }

                        if (score.Semester == "Học kỳ I")
                        {
                            sumSemester1 += scoreValue * (double)score.ScoreFactor;
                            countSemester1 += score.ScoreFactor;
                            allPassSemester1 = false;
                        }
                        else if (score.Semester == "Học kỳ II")
                        {
                            sumSemester2 += scoreValue * (double)score.ScoreFactor;
                            countSemester2 += score.ScoreFactor;
                            allPassSemester2 = false;
                        }

                        totalSum += scoreValue * (double)score.ScoreFactor;
                        totalCount += score.ScoreFactor;
                        allPassYear = false;
                    }
                }

                double averageSemester1 = countSemester1 > 0 ? (double)Math.Round(sumSemester1 / (double)countSemester1, 2) : 0;
                double averageSemester2 = countSemester2 > 0 ? (double)Math.Round(sumSemester2 / (double)countSemester2, 2) : 0;
                double averageYear = totalCount > 0 ? (double)Math.Round(totalSum / (double)totalCount, 2) : 0;

                string averageSemester1Str = hasNegativeOneScore_Sem1 ? "0" : (allPassSemester1 ? "Đ" : (countSemester1 == 0 ? "CĐ" : averageSemester1.ToString("F2")));
                string averageSemester2Str = hasNegativeOneScore_Sem2 ? "0" : (allPassSemester2 ? "Đ" : (countSemester2 == 0 ? "CĐ" : averageSemester2.ToString("F2")));
                string averageYearStr = hasNegativeOneScore_Year ? "0" : (allPassYear ? "Đ" : (totalCount == 0 ? "CĐ" : averageYear.ToString("F2")));

                if (!ranks.ContainsKey(averageYear))
                {
                    ranks[averageYear] = 0;
                }

                scores.Add(new ScoreResponse
                {
                    ID = studentClass.StudentID,
                    FullName = studentClass.AccountStudent.Student.Fullname,
                    AverageSemester1 = averageSemester1Str,
                    AverageSemester2 = averageSemester2Str,
                    AverageYear = averageYearStr,
                    Scores = scoreDetails
                });
            }

            Dictionary<double, int> uniqueDict = ranks.OrderByDescending(kvp => kvp.Key).Select((kvp, index) => new { kvp.Key, Rank = index + 1 }).ToDictionary(x => x.Key, x => x.Rank);

            foreach (var score in scores)
            {
                if (double.TryParse(score.AverageYear, out double avgYear))
                {
                    score.Rank = uniqueDict[avgYear];
                }
                else if (score.AverageYear == "Đ")
                {
                    score.Rank = 1; // Assuming rank 1 for all "Đ"
                }
                else
                {
                    score.Rank = uniqueDict.Values.Max() + 1; // Rank "CĐ" lowest
                }
            }

            return new ScoresResponse
            {
                Class = classes.Classroom,
                SchoolYear = schoolYear,
                Subject = subject.Name,
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
                    .Include(s => s.SchoolYear)
                    .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                    && s.Subject.ToLower().Equals(item.Name.ToLower())
                    && s.StudentID.ToLower().Equals(student.ID.ToLower())
                    && s.SchoolYear.Name.ToLower().Equals(classes.SchoolYear.Name.ToLower()))
                    .ToListAsync();

                List<ScoreDetailResponse> scoreDetails = studentScores
                    .Select(item1 => new ScoreDetailResponse()
                    {
                        Key = item1.Name,
                        Semester = item1.Semester,
                        Value = item1.Score,
                        IndexCol = item1.IndexColumn
                    })
                    .OrderBy(s => s.Semester)
                    .ThenBy(s => s.Key)
                    .ThenBy(s => s.IndexCol)
                    .ToList();

                double sum = 0;
                decimal count = 0;

                bool allPassYear = true;

                foreach (var item1 in studentScores)
                {
                    if (item1.Score.Equals("Đ", StringComparison.OrdinalIgnoreCase) || item1.Score.Equals("CĐ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (item1.Score != "Đ")
                        {
                            allPassYear = false;
                        }
                    }
                    else if (double.TryParse(item1.Score, out double score))
                    {
                        sum += score * (double)item1.ScoreFactor;
                        count += item1.ScoreFactor;
                        allPassYear = false;
                    }
                }

                string averageYearStr = allPassYear ? "Đ" : (count == 0 ? "CĐ" : (Math.Round(sum / (double)count, 2)).ToString());

                scores.Add(new ScoreSubjectResponse()
                {
                    Subject = item.Name,
                    Average = averageYearStr,
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
                    .Include(s => s.SchoolYear)
                    .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                    && s.Subject.ToLower().Equals(item.Name.ToLower())
                    && s.StudentID.ToLower().Equals(student.ID.ToLower())
                    && s.SchoolYear.Name.ToLower().Equals(classes.SchoolYear.Name.ToLower()))
                    .ToListAsync();

                List<ScoreDetailResponse> scoreDetails = studentScores
                    .Select(item1 => new ScoreDetailResponse()
                    {
                        Key = item1.Name,
                        Semester = item1.Semester,
                        Value = item1.Score,
                        IndexCol = item1.IndexColumn
                    })
                    .OrderBy(s => s.Semester)
                    .ThenBy(s => s.Key)
                    .ThenBy(s => s.IndexCol)
                    .ToList();

                double sum = 0;
                decimal count = 0;

                bool allPassYear = true;

                foreach (var item1 in studentScores)
                {
                    if (item1.Score.Equals("Đ", StringComparison.OrdinalIgnoreCase) || item1.Score.Equals("CĐ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (item1.Score != "Đ")
                        {
                            allPassYear = false;
                        }
                    }
                    else if (double.TryParse(item1.Score, out double score))
                    {
                        sum += score * (double)item1.ScoreFactor;
                        count += item1.ScoreFactor;
                        allPassYear = false;
                    }
                }

                string averageYearStr = allPassYear ? "Đ" : (count == 0 ? "CĐ" : (Math.Round(sum / (double)count, 2)).ToString());

                scores.Add(new ScoreSubjectResponse()
                {
                    Subject = item.Name,
                    Average = averageYearStr,
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

        public async Task<AverageScoresResponse> GetAverageScoresByClass(string className, string schoolYear)
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

            List<Subject> subjects = await _context.Subjects
                .Include(s => s.ComponentScores)
                .Include(s => s.Schedules)
                .Where(s => s.Schedules.Select(s => s.ClassID.ToString().ToLower()).Contains(classes.ID.ToString().ToLower())
                && s.ComponentScores.Count > 0)
                .ToListAsync();

            List<StudentScores> studentScores = await _context.StudentScores
                .Include(s => s.SchoolYear)
                .Where(s => classes.StudentClasses.Select(a => a.StudentID).Contains(s.StudentID)
                && s.SchoolYear.Name.ToLower().Equals(classes.SchoolYear.Name.ToLower()))
                .ToListAsync();

            List<AverageScoreResponse> averages = new List<AverageScoreResponse>();

            foreach (var studentClass in classes.StudentClasses)
            {
                var studentSubjectScores = studentScores
                    .Where(s => s.StudentID.ToLower().Equals(studentClass.StudentID.ToLower()))
                    .GroupBy(s => s.Subject);

                List<SubjectAverageResponse> subjectAverages = new List<SubjectAverageResponse>();

                foreach (var subjectGroup in studentSubjectScores)
                {
                    double subjectSumWholeYear = 0;
                    decimal subjectCountWholeYear = 0;
                    double subjectSumSemester1 = 0;
                    decimal subjectCountSemester1 = 0;
                    double subjectSumSemester2 = 0;
                    decimal subjectCountSemester2 = 0;
                    bool allScoresAreD = true;
                    bool allScoresAreD_Sem1 = true;
                    bool allScoresAreD_Sem2 = true;
                    bool hasNegativeOneScore = false;
                    bool hasNegativeOneScore_Sem1 = false;
                    bool hasNegativeOneScore_Sem2 = false;

                    foreach (var scoreItem in subjectGroup)
                    {
                        string score = scoreItem.Score.ToLower();
                        if (score == "đ" || score == "cđ")
                        {
                            if (score != "đ") allScoresAreD = false;
                            if (score != "đ" && scoreItem.Semester.Equals("Học kỳ I", StringComparison.OrdinalIgnoreCase)) allScoresAreD_Sem1 = false;
                            if (score != "đ" && scoreItem.Semester.Equals("Học kỳ II", StringComparison.OrdinalIgnoreCase)) allScoresAreD_Sem2 = false;
                        }
                        else if (double.TryParse(scoreItem.Score, out double numericScore))
                        {
                            if (numericScore == -1)
                            {
                                hasNegativeOneScore = true;
                                if (scoreItem.Semester.Equals("Học kỳ I", StringComparison.OrdinalIgnoreCase))
                                {
                                    hasNegativeOneScore_Sem1 = true;
                                }
                                else if (scoreItem.Semester.Equals("Học kỳ II", StringComparison.OrdinalIgnoreCase))
                                {
                                    hasNegativeOneScore_Sem2 = true;
                                }
                            }

                            subjectSumWholeYear += numericScore * (double)scoreItem.ScoreFactor;
                            subjectCountWholeYear += scoreItem.ScoreFactor;

                            if (scoreItem.Semester.Equals("Học kỳ I", StringComparison.OrdinalIgnoreCase))
                            {
                                subjectSumSemester1 += numericScore * (double)scoreItem.ScoreFactor;
                                subjectCountSemester1 += scoreItem.ScoreFactor;
                                allScoresAreD_Sem1 = false;
                            }
                            else if (scoreItem.Semester.Equals("Học kỳ II", StringComparison.OrdinalIgnoreCase))
                            {
                                subjectSumSemester2 += numericScore * (double)scoreItem.ScoreFactor;
                                subjectCountSemester2 += scoreItem.ScoreFactor;
                                allScoresAreD_Sem2 = false;
                            }
                            allScoresAreD = false;
                        }
                        else
                        {
                            throw new ArgumentException("Điểm không hợp lệ");
                        }
                    }

                    string averageWholeYear = allScoresAreD ? "Đ" : "CĐ";
                    string averageSemester1 = allScoresAreD_Sem1 ? "Đ" : "CĐ";
                    string averageSemester2 = allScoresAreD_Sem2 ? "Đ" : "CĐ";

                    if (hasNegativeOneScore)
                    {
                        averageWholeYear = "0";
                    }
                    else if (!allScoresAreD)
                    {
                        averageWholeYear = subjectCountWholeYear == 0 ? "CĐ" : (Math.Round(subjectSumWholeYear / (double)subjectCountWholeYear, 2)).ToString();
                    }

                    if (hasNegativeOneScore_Sem1)
                    {
                        averageSemester1 = "0";
                    }
                    else if (!allScoresAreD_Sem1)
                    {
                        averageSemester1 = subjectCountSemester1 == 0 ? "CĐ" : (Math.Round(subjectSumSemester1 / (double)subjectCountSemester1, 2)).ToString();
                    }

                    if (hasNegativeOneScore_Sem2)
                    {
                        averageSemester2 = "0";
                    }
                    else if (!allScoresAreD_Sem2)
                    {
                        averageSemester2 = subjectCountSemester2 == 0 ? "CĐ" : (Math.Round(subjectSumSemester2 / (double)subjectCountSemester2, 2)).ToString();
                    }

                    subjectAverages.Add(new SubjectAverageResponse()
                    {
                        Subject = subjectGroup.Key,
                        AverageWholeYear = averageWholeYear,
                        AverageSemester1 = averageSemester1,
                        AverageSemester2 = averageSemester2
                    });
                }

                averages.Add(new AverageScoreResponse()
                {
                    ID = studentClass.StudentID,
                    FullName = studentClass.AccountStudent.Student.Fullname,
                    SubjectAverages = subjectAverages
                });
            }

            return new AverageScoresResponse()
            {
                Class = classes.Classroom,
                SchoolYear = schoolYear,
                TeacherName = classes.Teacher.User.Fullname,
                Averages = averages
            };
        }

        public async Task<List<ScoreSubjectWithSemesterResponse>> GetScoresByStudentWithSemesters(string studentID, string schoolYear)
        {
            var student = await _context.AccountStudents
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.ID.ToLower() == studentID.ToLower() && a.IsActive)
                ?? throw new NotFoundException("Học sinh không tồn tại");

            var classes = await _context.Classes
                .Include(c => c.StudentClasses)
                .ThenInclude(c => c.AccountStudent)
                .ThenInclude(c => c.Student)
                .Include(c => c.SchoolYear)
                .FirstOrDefaultAsync(c => c.StudentClasses.Any(sc => sc.StudentID.ToLower() == studentID.ToLower())
                    && c.SchoolYear.Name.ToLower() == schoolYear.ToLower())
                ?? throw new NotFoundException("Lớp học không tồn tại");

            var subjects = await _context.Subjects
                .Include(s => s.ComponentScores)
                .Include(s => s.Schedules)
                .Where(s => s.Schedules.Any(sc => sc.ClassID == classes.ID) && s.ComponentScores.Any())
                .ToListAsync();

            var scoreSubjects = new List<ScoreSubjectWithSemesterResponse>();

            foreach (var subject in subjects)
            {
                var scores = await _context.StudentScores
                    .Include(ss => ss.SchoolYear)
                    .Where(ss => ss.StudentID.ToLower() == student.ID.ToLower()
                        && ss.Subject.ToLower() == subject.Name.ToLower()
                        && ss.SchoolYear.Name.ToLower() == schoolYear.ToLower())
                    .ToListAsync();

                var semester1Score = scores.Where(ss => ss.Semester.Equals("Học kỳ I")).ToList();
                var semester2Score = scores.Where(ss => ss.Semester.Equals("Học kỳ II")).ToList();

                var semester1Average = CalculateYearAverage(semester1Score);
                var semester2Average = CalculateYearAverage(semester2Score);
                var yearAverage = CalculateYearAverage(scores);

                scoreSubjects.Add(new ScoreSubjectWithSemesterResponse
                {
                    Subject = subject.Name,
                    Semester1Average = semester1Average,
                    Semester2Average = semester2Average,
                    YearAverage = yearAverage
                });
            }

            return scoreSubjects;
        }

        private string CalculateYearAverage(List<StudentScores> scores)
        {
            if (scores.All(ss => ss.Score.Equals("Đ", StringComparison.OrdinalIgnoreCase)))
                return "Đ";
            else if (scores.Any(ss => ss.Score.Equals("CĐ", StringComparison.OrdinalIgnoreCase)))
                return "CĐ";

            double sum = 0;
            decimal count = 0;

            foreach (var score in scores)
            {
                if (double.TryParse(score.Score, out double numericScore))
                {
                    sum += numericScore * (double)score.ScoreFactor;
                    count += score.ScoreFactor;
                }
            }

            if (count == 0)
                return "CĐ";
            else
                return Math.Round(sum / (double)count, 2).ToString();
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
                                                string score = data.ElementAt(i);

                                                // Convert the score to lowercase for comparison
                                                string lowerScore = score.ToLower();

                                                // Check if the score is a valid integer or one of the special cases
                                                if (int.TryParse(score, out int s))
                                                {
                                                    if (s < 0 || s > 10)
                                                    {
                                                        throw new ArgumentException("Điểm phải nằm trong thang điểm 10");
                                                    }
                                                }
                                                else if (lowerScore != "đ" && lowerScore != "cđ")
                                                {
                                                    throw new ArgumentException("Điểm phải nằm trong thang điểm 10 hoặc là Đ, đ, CĐ, cđ");
                                                }

                                                // Add the score to the dictionary
                                                strScores.Add(str.ToLower(), score);
                                            }
                                        }
                                        break;
                                }
                            }

                            Classes classes = await _context.Classes
                                .Include(c => c.SchoolYear)
                                .Include(c => c.StudentClasses)
                                .ThenInclude(c => c.AccountStudent)
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
                                .Include(s => s.SchoolYear)
                                    .ThenInclude(sy => sy.Classes)
                                .Where(s => s.SchoolYearID == classes.SchoolYearID
                                    && s.Subject.ToLower() == subject.Name.ToLower()
                                    && s.Name.ToLower() == componentScore.Name.ToLower()
                                    && s.IndexColumn == indexCol
                                    && s.Semester.ToLower() == strSemester.ToLower()
                                    && classes.StudentClasses.Select(a => a.AccountStudent.ID).Contains(s.StudentID))
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
