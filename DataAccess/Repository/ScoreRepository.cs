using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using ClosedXML.Excel;
using DataAccess.Context;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IEmailSender _emailSender;

        public ScoreRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository,
            IEmailSender emailSender)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
            _emailSender = emailSender;
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

            foreach (var studentClass in classes.StudentClasses)
            {
                var studentScoresBySubject = studentScores
                    .Where(s => s.StudentID.ToLower().Equals(studentClass.StudentID.ToLower()))
                    .ToList();

                var scoreDetails = studentScoresBySubject
                    .OrderBy(s => s.ScoreFactor)
                    .Select(s => {
                        double scoreDouble;
                        var isDouble = double.TryParse(s.Score, out scoreDouble);
                        return new ScoreDetailResponse
                        {
                            Key = s.Name,
                            Semester = s.Semester,
                            Value = isDouble ? Math.Round(scoreDouble, 1).ToString("F1") : s.Score,
                            IndexCol = s.IndexColumn
                        };
                    })
                    .OrderBy(s => s.Semester)
                    .ThenBy(s => s.Key)
                    .ThenBy(s => s.IndexCol)
                    .ToList();

                double sumSemester1 = 0, sumSemester2 = 0, totalSum = 0;
                decimal countSemester1 = 0, countSemester2 = 0, totalCount = 0;

                bool hasNegativeOneScore = false;
                bool hasCDScoreSemester1 = false;
                bool hasCDScoreSemester2 = false;
                bool allDScoresSemester1 = true;
                bool allDScoresSemester2 = true;

                foreach (var score in studentScoresBySubject)
                {
                    if (score.Score == "CĐ")
                    {
                        if (score.Semester == "Học kỳ I")
                        {
                            hasCDScoreSemester1 = true;
                        }
                        else if (score.Semester == "Học kỳ II")
                        {
                            hasCDScoreSemester2 = true;
                        }
                    }
                    else if (double.TryParse(score.Score, out double scoreValue))
                    {
                        if (scoreValue == -1)
                        {
                            hasNegativeOneScore = true;
                            break;
                        }

                        if (score.Semester == "Học kỳ I")
                        {
                            sumSemester1 += scoreValue * (double)score.ScoreFactor;
                            countSemester1 += score.ScoreFactor;
                            allDScoresSemester1 = false;
                        }
                        else if (score.Semester == "Học kỳ II")
                        {
                            sumSemester2 += scoreValue * (double)score.ScoreFactor;
                            countSemester2 += score.ScoreFactor;
                            allDScoresSemester2 = false;
                        }

                        totalSum += scoreValue * (double)score.ScoreFactor;
                        totalCount += score.ScoreFactor;
                    }
                    else if (score.Score != "Đ")
                    {
                        if (score.Semester == "Học kỳ I")
                        {
                            allDScoresSemester1 = false;
                        }
                        else if (score.Semester == "Học kỳ II")
                        {
                            allDScoresSemester2 = false;
                        }
                    }
                }

                string averageSemester1Str = hasNegativeOneScore ? "0" : (hasCDScoreSemester1 ? "CĐ" : (allDScoresSemester1 ? "Đ" : ((double)Math.Round(sumSemester1 / (double)countSemester1, 1)).ToString("F1")));
                string averageSemester2Str = hasNegativeOneScore ? "0" : (hasCDScoreSemester2 ? "CĐ" : (allDScoresSemester2 ? "Đ" : ((double)Math.Round(sumSemester2 / (double)countSemester2, 1)).ToString("F1")));
                string averageYearStr = hasNegativeOneScore ? "0" : (hasCDScoreSemester1 || hasCDScoreSemester2 ? "CĐ" : (allDScoresSemester1 && allDScoresSemester2 ? "Đ" : ((double)Math.Round(totalSum / (double)totalCount, 1)).ToString("F1")));

                scores.Add(new ScoreResponse
                {
                    ID = studentClass.StudentID,
                    FullName = studentClass.AccountStudent.Student.Fullname,
                    AverageSemester1 = averageSemester1Str,
                    AverageSemester2 = averageSemester2Str,
                    AverageYear = averageYearStr,
                    Scores = scoreDetails,
                    Rank = hasNegativeOneScore ? 1 : 0 // Set rank 1 if any score is -1
                });
            }

            if (scores.All(s => s.Rank == 1))
            {
                return new ScoresResponse
                {
                    Class = classes.Classroom,
                    SchoolYear = schoolYear,
                    Subject = subject.Name,
                    TeacherName = classes.Teacher.User.Fullname,
                    Score = scores
                };
            }

            Dictionary<double, int> ranks = new();
            foreach (var score in scores)
            {
                if (double.TryParse(score.AverageYear, out double avgYear))
                {
                    if (!ranks.ContainsKey(avgYear))
                    {
                        ranks[avgYear] = 0;
                    }
                }
            }

            Dictionary<double, int> uniqueDict = ranks.OrderByDescending(kvp => kvp.Key).Select((kvp, index) => new { kvp.Key, Rank = index + 1 }).ToDictionary(x => x.Key, x => x.Rank);

            foreach (var score in scores)
            {
                if (double.TryParse(score.AverageYear, out double avgYear))
                {
                    score.Rank = uniqueDict[avgYear];
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

                string averageYearStr = allPassYear ? "Đ" : (count == 0 ? "CĐ" : (Math.Round(sum / (double)count, 1)).ToString());

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
                    .OrderBy(s => s.ScoreFactor)
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

                string averageYearStr = allPassYear ? "Đ" : (count == 0 ? "CĐ" : (Math.Round(sum / (double)count, 1)).ToString());

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
            // Fetch class information
            var classes = await _context.Classes
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.AccountStudent)
                        .ThenInclude(a => a.Student)
                .Include(c => c.SchoolYear)
                .Include(c => c.Teacher)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(c => c.Classroom.ToLower() == className.ToLower()
                                       && c.SchoolYear.Name.ToLower() == schoolYear.ToLower())
                ?? throw new NotFoundException("Lớp học không tồn tại");

            // Fetch student scores
            var studentScores = await _context.StudentScores
                .Include(ss => ss.SchoolYear)
                .Where(ss => classes.StudentClasses.Select(sc => sc.StudentID).Contains(ss.StudentID)
                           && ss.SchoolYear.Name.ToLower() == schoolYear.ToLower())
                .ToListAsync();

            var averages = new List<AverageScoreResponse>();

            foreach (var studentClass in classes.StudentClasses)
            {
                var studentScoresForStudent = studentScores
                    .Where(ss => ss.StudentID == studentClass.StudentID)
                    .GroupBy(ss => ss.Subject);

                var subjectAverages = new List<SubjectAverageResponse>();
                double totalSumWholeYear = 0;
                decimal totalCountWholeYear = 0;
                double totalSumSemester1 = 0;
                decimal totalCountSemester1 = 0;
                double totalSumSemester2 = 0;
                decimal totalCountSemester2 = 0;

                bool allSubjectsAbove65WholeYear = true;
                bool allSubjectsAbove5WholeYear = true;
                bool allSubjectsAbove65Semester1 = true;
                bool allSubjectsAbove5Semester1 = true;
                bool allSubjectsAbove65Semester2 = true;
                bool allSubjectsAbove5Semester2 = true;

                foreach (var subjectGroup in studentScoresForStudent)
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
                        if (scoreItem.Score.ToLower() == "đ" || scoreItem.Score.ToLower() == "cđ")
                        {
                            if (scoreItem.Score.ToLower() != "đ") allScoresAreD = false;
                            if (scoreItem.Score.ToLower() != "đ" && scoreItem.Semester == "Học kỳ I") allScoresAreD_Sem1 = false;
                            if (scoreItem.Score.ToLower() != "đ" && scoreItem.Semester == "Học kỳ II") allScoresAreD_Sem2 = false;
                        }
                        else if (double.TryParse(scoreItem.Score, out var numericScore))
                        {
                            if (numericScore == -1)
                            {
                                hasNegativeOneScore = true;
                                if (scoreItem.Semester == "Học kỳ I") hasNegativeOneScore_Sem1 = true;
                                if (scoreItem.Semester == "Học kỳ II") hasNegativeOneScore_Sem2 = true;
                            }

                            subjectSumWholeYear += numericScore * (double)scoreItem.ScoreFactor;
                            subjectCountWholeYear += scoreItem.ScoreFactor;

                            if (scoreItem.Semester == "Học kỳ I")
                            {
                                subjectSumSemester1 += numericScore * (double)scoreItem.ScoreFactor;
                                subjectCountSemester1 += scoreItem.ScoreFactor;
                                allScoresAreD_Sem1 = false;
                            }
                            else if (scoreItem.Semester == "Học kỳ II")
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
                        averageWholeYear = subjectCountWholeYear == 0 ? "CĐ" : (Math.Round(subjectSumWholeYear / (double)subjectCountWholeYear, 1)).ToString();
                    }

                    if (hasNegativeOneScore_Sem1)
                    {
                        averageSemester1 = "0";
                    }
                    else if (!allScoresAreD_Sem1)
                    {
                        averageSemester1 = subjectCountSemester1 == 0 ? "CĐ" : (Math.Round(subjectSumSemester1 / (double)subjectCountSemester1, 1)).ToString();
                    }

                    if (hasNegativeOneScore_Sem2)
                    {
                        averageSemester2 = "0";
                    }
                    else if (!allScoresAreD_Sem2)
                    {
                        averageSemester2 = subjectCountSemester2 == 0 ? "CĐ" : (Math.Round(subjectSumSemester2 / (double)subjectCountSemester2, 1)).ToString();
                    }

                    // Check all subjects' average scores for the current subject group
                    if (double.TryParse(averageWholeYear, out var avgWholeYear))
                    {
                        if (avgWholeYear < 6.5) allSubjectsAbove65WholeYear = false;
                        if (avgWholeYear < 5) allSubjectsAbove5WholeYear = false;
                    }
                    if (double.TryParse(averageSemester1, out var avgSemester1))
                    {
                        if (avgSemester1 < 6.5) allSubjectsAbove65Semester1 = false;
                        if (avgSemester1 < 5) allSubjectsAbove5Semester1 = false;
                    }
                    if (double.TryParse(averageSemester2, out var avgSemester2))
                    {
                        if (avgSemester2 < 6.5) allSubjectsAbove65Semester2 = false;
                        if (avgSemester2 < 5) allSubjectsAbove5Semester2 = false;
                    }

                    subjectAverages.Add(new SubjectAverageResponse()
                    {
                        Subject = subjectGroup.Key,
                        AverageWholeYear = averageWholeYear,
                        AverageSemester1 = averageSemester1,
                        AverageSemester2 = averageSemester2
                    });

                    // Add to total sums
                    if (double.TryParse(averageWholeYear, out avgWholeYear))
                    {
                        totalSumWholeYear += avgWholeYear;
                        totalCountWholeYear++;
                    }
                    if (double.TryParse(averageSemester1, out avgSemester1))
                    {
                        totalSumSemester1 += avgSemester1;
                        totalCountSemester1++;
                    }
                    if (double.TryParse(averageSemester2, out avgSemester2))
                    {
                        totalSumSemester2 += avgSemester2;
                        totalCountSemester2++;
                    }
                }

                string totalAverageWholeYear = totalCountWholeYear == 0 ? "CĐ" : (Math.Round(totalSumWholeYear / (double)totalCountWholeYear, 1)).ToString();
                string totalAverageSemester1 = totalCountSemester1 == 0 ? "CĐ" : (Math.Round(totalSumSemester1 / (double)totalCountSemester1, 1)).ToString();
                string totalAverageSemester2 = totalCountSemester2 == 0 ? "CĐ" : (Math.Round(totalSumSemester2 / (double)totalCountSemester2, 1)).ToString();

                // Determine academic performance
                string performanceWholeYear = GetAcademicPerformance(Math.Round(totalSumWholeYear / (double)totalCountWholeYear, 1), allSubjectsAbove65WholeYear, allSubjectsAbove5WholeYear);
                string performanceSemester1 = GetAcademicPerformance(Math.Round(totalSumSemester1 / (double)totalCountSemester1, 1), allSubjectsAbove65Semester1, allSubjectsAbove5Semester1);
                string performanceSemester2 = GetAcademicPerformance(Math.Round(totalSumSemester2 / (double)totalCountSemester2, 1), allSubjectsAbove65Semester2, allSubjectsAbove5Semester2);

                averages.Add(new AverageScoreResponse()
                {
                    ID = studentClass.StudentID,
                    FullName = studentClass.AccountStudent.Student.Fullname,
                    SubjectAverages = subjectAverages,
                    TotalAverageWholeYear = totalAverageWholeYear,
                    TotalAverageSemester1 = totalAverageSemester1,
                    TotalAverageSemester2 = totalAverageSemester2,
                    PerformanceWholeYear = performanceWholeYear,
                    PerformanceSemester1 = performanceSemester1,
                    PerformanceSemester2 = performanceSemester2
                });
            }

            // Rank the students
            var rankedAverages = averages.OrderByDescending(a => double.TryParse(a.TotalAverageWholeYear, out var avgWholeYear) ? avgWholeYear : 0)
                                         .ThenByDescending(a => double.TryParse(a.TotalAverageSemester1, out var avgSem1) ? avgSem1 : 0)
                                         .ThenByDescending(a => double.TryParse(a.TotalAverageSemester2, out var avgSem2) ? avgSem2 : 0)
                                         .ToList();

            // Tạo danh sách các điểm trung bình duy nhất và sắp xếp từ lớn đến bé
            var uniqueWholeYearAverages = rankedAverages.Select(a => double.TryParse(a.TotalAverageWholeYear, out var avgWholeYear) ? avgWholeYear : 0)
                                                        .Distinct()
                                                        .OrderByDescending(avg => avg)
                                                        .ToList();

            var uniqueSemester1Averages = rankedAverages.Select(a => double.TryParse(a.TotalAverageSemester1, out var avgSem1) ? avgSem1 : 0)
                                                        .Distinct()
                                                        .OrderByDescending(avg => avg)
                                                        .ToList();

            var uniqueSemester2Averages = rankedAverages.Select(a => double.TryParse(a.TotalAverageSemester2, out var avgSem2) ? avgSem2 : 0)
                                                        .Distinct()
                                                        .OrderByDescending(avg => avg)
                                                        .ToList();

            // Gán hạng dựa trên vị trí trong danh sách đã sắp xếp
            foreach (var student in rankedAverages)
            {
                if (double.TryParse(student.TotalAverageWholeYear, out var avgWholeYear))
                {
                    student.RankWholeYear = uniqueWholeYearAverages.IndexOf(avgWholeYear) + 1;
                }
                if (double.TryParse(student.TotalAverageSemester1, out var avgSem1))
                {
                    student.RankSemester1 = uniqueSemester1Averages.IndexOf(avgSem1) + 1;
                }
                if (double.TryParse(student.TotalAverageSemester2, out var avgSem2))
                {
                    student.RankSemester2 = uniqueSemester2Averages.IndexOf(avgSem2) + 1;
                }
            }

            return new AverageScoresResponse()
            {
                Class = classes.Classroom,
                SchoolYear = schoolYear,
                TeacherName = classes.Teacher.User.Fullname,
                Averages = rankedAverages.OrderBy(a => a.ID).ToList(),
            };
        }

        private string GetAcademicPerformance(double averageScore, bool allSubjectsAbove65, bool allSubjectsAbove5)
        {
            if (averageScore >= 8 && allSubjectsAbove65)
            {
                return "Giỏi";
            }
            else if (averageScore >= 6.5 && allSubjectsAbove5)
            {
                return "Khá";
            }
            else if (averageScore >= 5)
            {
                return "Trung Bình";
            }
            else if (averageScore >= 3.5)
            {
                return "Yếu";
            }
            else
            {
                return "Kém";
            }
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

        public async Task UpdateScoreByExcel(string accountID, ExcelRequest request)
        {
            Account account = await _context.Accounts
                .Include(a => a.AccountRoles)
                .ThenInclude(a => a.Role)
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

                                                string lowerScore = score.ToLower();

                                                if (double.TryParse(score, out double s))
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

                                                strScores.Add(str.ToLower(), score.ToUpper());
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

                            if (!account.AccountRoles.Any(ar => ar.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                            {
                                Schedule checkPer = await _context.Schedules
                                    .FirstOrDefaultAsync(s => s.TeacherID.ToLower().Equals(account.ID) && Guid.Equals(s.SubjectID, subject.ID) && Guid.Equals(s.ClassID, classes.ID))
                                    ?? throw new NotFoundException(account.Username + " không có quyền nhập điểm môn " + subject.Name + " của lớp " + classes.Classroom + " năm học " + classes.SchoolYear.Name);
                            }

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

                            string log = "Người dùng " + account.Username + " vừa thực hiện cập nhật điểm " + strScore.ToLower() + " lần thứ " + indexCol + " môn " + subject.Name + " của lớp " + classes.Classroom + " năm học " + classes.SchoolYear.Name;

                            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
                            {
                                AccountID = accountID,
                                Note = log,
                                Type = LogName.UPDATE.ToString(),
                            });

                            SchoolSetting school = await _context.SchoolSettings.FirstOrDefaultAsync();

                            if (school != null)
                            {
                                await _emailSender.SendEmailAsync(school.SchoolEmail, "Thông báo cập nhật điểm",
                                    log);
                            }
                        }
                    }
                }
            }
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
                return Math.Round(sum / (double)count, 1).ToString();
        }

        private string CalculateAcademicPerformance(double overallAverage, List<double> subjectAverages)
        {
            if (subjectAverages.Any(s => s < 5))
                return "Yếu";
            if (overallAverage < 5)
                return "Yếu";
            if (overallAverage < 6.5)
                return "Trung bình";
            if (overallAverage >= 8 && subjectAverages.All(s => s >= 6.5))
                return "Giỏi";
            if (overallAverage >= 6.5 && subjectAverages.All(s => s >= 5))
                return "Khá";

            return "Trung bình";
        }
    }
}
