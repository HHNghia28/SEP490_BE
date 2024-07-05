﻿using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly ApplicationDbContext _context;

        public StatisticRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StatisticAttendenceResponse>> GetStatisticAttendance(string schoolYear, int grade = 0, string fromDate = null, string toDate = null)
        {
            var currentDate = DateTime.Now;

            DateTime? parsedFromDate = null;
            DateTime? parsedToDate = null;

            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                var firstDayOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);

                parsedFromDate = firstDayOfWeek;

                parsedToDate = firstDayOfWeek.AddDays(6);
            }
            else
            {
                if (DateTime.TryParseExact(fromDate, new[] { "dd/MM/yyyy", "d/M/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFrom))
                {
                    parsedFromDate = parsedFrom;
                }

                if (DateTime.TryParseExact(toDate, new[] { "dd/MM/yyyy", "d/M/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo))
                {
                    parsedToDate = parsedTo;
                }
            }

            // Fetch schedules and related data
            var classSchedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                .ThenInclude(s => s.Teacher)
                .Include(s => s.Teacher)
                .Where(s => s.Classes.SchoolYear.Name == schoolYear && s.Classes.IsActive && (!parsedFromDate.HasValue || s.Date >= parsedFromDate.Value) &&
                            (!parsedToDate.HasValue || s.Date <= parsedToDate.Value))
                .Select(s => new
                {
                    s.ID,
                    s.Classes.Classroom,
                    TeacherName = s.Classes.Teacher.Username,
                    s.Date,
                    Grade = s.Classes.Classroom.Length >= 2 ? int.Parse(s.Classes.Classroom.Substring(0, 2)) : (int?)null
                })
                .ToListAsync();

            // Filter schedules by grade
            var filteredSchedules = classSchedules
                .Where(s => grade == 0 || s.Grade == grade)
                .ToList();

            // Get the IDs of the filtered schedules
            var scheduleIds = filteredSchedules.Select(s => s.ID).ToList();

            // Fetch attendances for the filtered schedules
            var attendances = await _context.Attendances
                .AsNoTracking()
                .Where(a => scheduleIds.Contains(a.ScheduleID))
                .Select(a => new
                {
                    a.ScheduleID,
                    a.Present,
                    a.Date,
                    a.StudentID
                })
                .ToListAsync();

            // Group and process the data, aggregating by classroom name
            var groupedData = filteredSchedules
                .GroupBy(cs => new
                {
                    cs.Classroom,
                    cs.TeacherName
                })
                .Select(g => new StatisticAttendenceResponse
                {
                    ClassName = g.Key.Classroom,
                    Grade = grade == 0 ? g.Key.Classroom.Substring(0, 2) : grade.ToString(),
                    Teacher = g.Key.TeacherName,
                    NumberOfStudent = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID)).Select(a => a.StudentID).Distinct().Count(),
                    NumberOfPresent = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID) && a.Date <= currentDate && a.Present).Count(),
                    NumberOfAbsent = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID) && a.Date <= currentDate && !a.Present).Count(),
                    NumberOfNotYet = attendances.Where(a => filteredSchedules.Where(fs => fs.Classroom == g.Key.Classroom).Select(fs => fs.ID).Contains(a.ScheduleID) && a.Date > currentDate).Count()
                })
                .OrderBy(g => g.ClassName)
                .ToList();

            return groupedData;
        }

        public async Task<IEnumerable<ScoreStatisticsResponse>> GetScoreStatistics(string schoolYear, string className = null, int grade = 0, string subject = null)
        {
            var classesQuery = _context.Classes
                .AsNoTracking()
                .Include(c => c.SchoolYear)
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.AccountStudent)
                        .ThenInclude(sa => sa.Scores)
                .Where(c => c.SchoolYear.Name == schoolYear);

            if(!string.IsNullOrEmpty(subject))
            {
                classesQuery = _context.Classes
                .AsNoTracking()
                .Include(c => c.SchoolYear)
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.AccountStudent)
                        .ThenInclude(sa => sa.Scores.Where(s => s.Subject == subject))
                .Where(c => c.SchoolYear.Name == schoolYear);
            }

            if (!string.IsNullOrEmpty(className))
            {
                classesQuery = classesQuery.Where(c => c.Classroom.ToLower() == className.ToLower());
            }

            if (grade != 0)
            {
                classesQuery = classesQuery.Where(c => c.Classroom.StartsWith(grade.ToString()));
            }

            var classes = await classesQuery.ToListAsync();


            var scoreStatistics = classes
                .SelectMany(c => c.StudentClasses, (c, sc) => new { c.Classroom, sc.AccountStudent.Scores })
                .SelectMany(cs => cs.Scores.Select(s => new
                {
                    Grade = cs.Classroom.Length >= 2 ? int.Parse(cs.Classroom.Substring(0, 2)) : (int?)null,
                    s.Score,
                    s.Name,
                    s.Semester
                }))
                .Where(s => s.Grade.HasValue && !string.IsNullOrEmpty(s.Score) && s.Score != "-1")
                .GroupBy(s => s.Grade.Value)
                .Select(g => new ScoreStatisticsResponse
                {
                    Grade = g.Key,
                    Semesters = new List<SemesterScore>
                    {
                new SemesterScore
                {
                    Name = "Học kỳ I",
                    Scores = g
                        .Where(s => s.Semester == "Học kỳ I")
                        .GroupBy(s => s.Name)
                        .Select(ng => new NameScore
                        {
                            Name = ng.Key,
                            ScoreCounts = ng
                                .GroupBy(s => s.Score)
                                .OrderBy(sg => ParseScoreKey(sg.Key)) 
                                .Select(sg => new CountScore
                                {
                                    Key = sg.Key,
                                    Value = sg.Count().ToString()
                                })
                                .ToList()
                        })
                        .ToList()
                },
                new SemesterScore
                {
                    Name = "Học kỳ II",
                    Scores = g
                        .Where(s => s.Semester == "Học kỳ II")
                        .GroupBy(s => s.Name)
                        .Select(ng => new NameScore
                        {
                            Name = ng.Key,
                            ScoreCounts = ng
                                .GroupBy(s => s.Score)
                                .OrderBy(sg => ParseScoreKey(sg.Key)) 
                                .Select(sg => new CountScore
                                {
                                    Key = sg.Key,
                                    Value = sg.Count().ToString()
                                })
                                .ToList()
                        })
                        .ToList()
                },
                new SemesterScore
                {
                    Name = "Cả năm",
                    Scores = g
                        .GroupBy(s => s.Name)
                        .Select(ng => new NameScore
                        {
                            Name = ng.Key,
                            ScoreCounts = ng
                                .GroupBy(s => s.Score)
                                .OrderBy(sg => ParseScoreKey(sg.Key)) 
                                .Select(sg => new CountScore
                                {
                                    Key = sg.Key,
                                    Value = sg.Count().ToString()
                                })
                                .ToList()
                        })
                        .ToList()
                }
                    }
                })
                .ToList();

            return scoreStatistics;
        }

        public async Task<IEnumerable<ScoreGradeStatisticsResponse>> GetGradeStatistics(string schoolYear, string className = null, int grade = 0)
        {
            var classesQuery = _context.Classes
                .AsNoTracking()
                .Include(c => c.SchoolYear)
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.AccountStudent)
                        .ThenInclude(sa => sa.Scores)
                .Where(c => c.SchoolYear.Name == schoolYear);

            if (!string.IsNullOrEmpty(className))
            {
                classesQuery = classesQuery.Where(c => c.Classroom.ToLower() == className.ToLower());
            }

            if (grade != 0)
            {
                classesQuery = classesQuery.Where(c => c.Classroom.StartsWith(grade.ToString()));
            }

            var classes = await classesQuery.ToListAsync();

            var studentScores = classes
                .SelectMany(c => c.StudentClasses, (c, sc) => new { sc.AccountStudent, Scores = sc.AccountStudent.Scores })
                .Select(s => new
                {
                    s.AccountStudent,
                    Scores = s.Scores.Select(score => new
                    {
                        score.Score,
                        score.ScoreFactor,
                        IsNumeric = double.TryParse(score.Score, out var _)
                    }).ToList()
                })
                .ToList();

            var averageScores = studentScores
                .Select(s => new
                {
                    s.AccountStudent,
                    AverageScore = CalculateAverageScore(s.Scores)
                })
                .ToList();

            var scoreStatistics = averageScores
                .GroupBy(s => s.AverageScore)
                .Select(g => new ScoreGradeStatisticsResponse
                {
                    AverageScore = g.Key,
                    Count = g.Count()
                })
                .OrderBy(s => ParseScoreKey(s.AverageScore))
                .ToList();

            return scoreStatistics;
        }

        public async Task<IEnumerable<ScoreAverageStatisticsResponse>> GetScoreAverageStatistics(string schoolYear, string className = null, int grade = 0)
        {
            var classesQuery = _context.Classes
                .AsNoTracking()
                .Include(c => c.SchoolYear)
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.AccountStudent)
                        .ThenInclude(sa => sa.Scores)
                .Where(c => c.SchoolYear.Name == schoolYear);

            if (!string.IsNullOrEmpty(className))
            {
                classesQuery = classesQuery.Where(c => c.Classroom.ToLower() == className.ToLower());
            }

            if (grade != 0)
            {
                classesQuery = classesQuery.Where(c => c.Classroom.StartsWith(grade.ToString()));
            }

            var classes = await classesQuery.ToListAsync();

            var studentScores = classes
                .SelectMany(c => c.StudentClasses, (c, sc) => new { sc.AccountStudent, Scores = sc.AccountStudent.Scores })
                .Select(s => new
                {
                    s.AccountStudent,
                    Scores = s.Scores.Select(score => new
                    {
                        score.Score,
                        score.ScoreFactor,
                        score.Semester,
                        IsNumeric = double.TryParse(score.Score, out var _)
                    }).ToList()
                })
                .ToList();

            var semesterScores = new List<ScoreAverageStatisticsResponse>
                {
                    CalculateAverageScores(studentScores, "Học kỳ I"),
                    CalculateAverageScores(studentScores, "Học kỳ II"),
                    CalculateAverageScores(studentScores, "Cả năm", true)
                };

            return semesterScores;
        }

        private ScoreAverageStatisticsResponse CalculateAverageScores(IEnumerable<dynamic> studentScores, string semester, bool isWholeYear = false)
        {
            var averageScores = studentScores
                .Select(s => new
                {
                    s.AccountStudent,
                    AverageScore = CalculateAverageScore(isWholeYear
                        ? s.Scores
                        : ((Func<IEnumerable<dynamic>>)(() => ((IEnumerable<dynamic>)s.Scores).Where(score => score.Semester == semester)))()
                    )
                })
                .GroupBy(s => s.AverageScore)
                .Select(g => new ScoreGradeStatisticsResponse
                {
                    AverageScore = g.Key,
                    Count = g.Count()
                })
                .OrderBy(s => ParseScoreKey(s.AverageScore))
                .ToList();

            return new ScoreAverageStatisticsResponse
            {
                Semester = semester,
                Scores = averageScores
            };
        }

        public async Task<IEnumerable<ScoreAverageStatisticsResponse>> GetGroupScoreAverageStatistics(string schoolYear, string className = null, int grade = 0)
        {
            var classesQuery = _context.Classes
                .AsNoTracking()
                .Include(c => c.SchoolYear)
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.AccountStudent)
                        .ThenInclude(sa => sa.Scores)
                .Where(c => c.SchoolYear.Name == schoolYear);

            if (!string.IsNullOrEmpty(className))
            {
                classesQuery = classesQuery.Where(c => c.Classroom.ToLower() == className.ToLower());
            }

            if (grade != 0)
            {
                classesQuery = classesQuery.Where(c => c.Classroom.StartsWith(grade.ToString()));
            }

            var classes = await classesQuery.ToListAsync();

            var studentScores = classes
                .SelectMany(c => c.StudentClasses, (c, sc) => new { sc.AccountStudent, Scores = sc.AccountStudent.Scores })
                .Select(s => new
                {
                    s.AccountStudent,
                    Scores = s.Scores.Select(score => new
                    {
                        score.Score,
                        score.ScoreFactor,
                        score.Semester,
                        IsNumeric = double.TryParse(score.Score, out var _)
                    }).ToList()
                })
                .ToList();

            var semesterScores = new List<ScoreAverageStatisticsResponse>
                {
                    CalculateAverageGroupScores(studentScores, "Học kỳ I"),
                    CalculateAverageGroupScores(studentScores, "Học kỳ II"),
                    CalculateAverageGroupScores(studentScores, "Cả năm", true)
                };

            return semesterScores;
        }

        private ScoreAverageStatisticsResponse CalculateAverageGroupScores(IEnumerable<dynamic> studentScores, string semester, bool isWholeYear = false)
        {
            var averageScores = studentScores
                .Select(s => new
                {
                    s.AccountStudent,
                    AverageScore = CalculateAverageScore(isWholeYear
                        ? s.Scores
                        : ((Func<IEnumerable<dynamic>>)(() => ((IEnumerable<dynamic>)s.Scores).Where(score => score.Semester == semester)))()
                    )
                })
                .GroupBy(s => CategorizeAverageScore(s.AverageScore))
                .Select(g => new ScoreGradeStatisticsResponse
                {
                    AverageScore = g.Key,
                    Count = g.Count()
                })
                .OrderBy(s => ParseGroupScoreKey(s.AverageScore))
                .ToList();

            return new ScoreAverageStatisticsResponse
            {
                Semester = semester,
                Scores = averageScores
            };
        }

        private string CalculateAverageScore(IEnumerable<dynamic> scores)
        {
            if (scores.Any(s => s.Score == "CĐ"))
            {
                return "CĐ";
            }

            if (scores.All(s => s.Score == "Đ"))
            {
                return "Đ";
            }

            decimal totalScore = 0;
            decimal totalFactor = 0;

            foreach (var score in scores)
            {
                if (score.IsNumeric)
                {
                    totalScore += (decimal)double.Parse(score.Score) * score.ScoreFactor;
                    totalFactor += score.ScoreFactor;
                }
            }

            if (totalFactor == 0)
            {
                return "N/A";
            }

            decimal averageScore = totalScore / totalFactor;
            return Math.Round(averageScore, 2).ToString(CultureInfo.InvariantCulture);
        }

        private int ParseScoreKey(string scoreKey)
        {
            if (scoreKey == "Đ") return 1000;
            if (scoreKey == "CĐ") return 1001;

            if (double.TryParse(scoreKey, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                int n = (int)(result * 100);
                return n;
            }

            return 1002;
        }

        private string CategorizeAverageScore(string averageScore)
        {
            if (averageScore == "Đ" || averageScore == "CĐ" || averageScore == "N/A")
            {
                return averageScore;
            }

            if (double.TryParse(averageScore, out var score))
            {
                if (score >= 0 && score < 1) return "0-1";
                if (score >= 1 && score < 2) return "1-2";
                if (score >= 2 && score < 3) return "2-3";
                if (score >= 3 && score < 4) return "3-4";
                if (score >= 4 && score < 5) return "4-5";
                if (score >= 5 && score < 6) return "5-6";
                if (score >= 6 && score < 7) return "6-7";
                if (score >= 7 && score < 8) return "7-8";
                if (score >= 8 && score < 9) return "8-9";
                if (score >= 9 && score < 10) return "9-10";
            }

            return "N/A";
        }

        private int ParseGroupScoreKey(string scoreKey)
        {
            if (scoreKey == "Đ") return 1000;
            if (scoreKey == "CĐ") return 1001;
            if (scoreKey == "N/A") return 1002;
            if (scoreKey == "0-1") return 0;
            if (scoreKey == "1-2") return 1;
            if (scoreKey == "2-3") return 2;
            if (scoreKey == "3-4") return 3;
            if (scoreKey == "4-5") return 4;
            if (scoreKey == "5-6") return 5;
            if (scoreKey == "6-7") return 6;
            if (scoreKey == "7-8") return 7;
            if (scoreKey == "8-9") return 8;
            if (scoreKey == "9-10") return 9;

            if (double.TryParse(scoreKey, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                int n = (int)(result * 100);
                return n;
            }

            return 1003;
        }
    }
}
