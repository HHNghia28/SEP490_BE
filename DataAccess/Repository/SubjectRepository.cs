using Azure.Core;
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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;

        public SubjectRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<IEnumerable<SubjectsResponse>> GetSubjects()
        {
            return await _context.Subjects
                .Include(s => s.ComponentScores)
                .Where(s => s.IsActive)
                .Select(item => new SubjectsResponse()
                {
                    ID = item.ID,
                    Grade = item.Grade,
                    Name = item.Name,
                    IsMark = item.ComponentScores.Any()
                })
                .OrderBy(s => s.Grade)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<SubjectResponse> GetSubject(string subjectID)
        {
            Subject subjectExist = await _context.Subjects
                        .Include(s => s.LessonPlans)
                        .Include(s => s.ComponentScores)
                        .FirstOrDefaultAsync(s => s.ID.ToString().ToLower().Equals(subjectID.ToLower().Trim()) && s.IsActive);

            if (subjectExist == null)
            {
                throw new NotFoundException("Không tìm thấy môn học");
            }

            List<ComponentScoreResponse> componentScores = new();

            foreach (var item in subjectExist.ComponentScores)
            {
                componentScores.Add(new ComponentScoreResponse()
                {
                    ID = item.ID,
                    Count = item.Count,
                    Name = item.Name,
                    ScoreFactor = item.ScoreFactor,
                    Semester = item.Semester,
                });
            }

            List<LessonPlanResponse> lessonPlans = new();

            foreach (var item in subjectExist.LessonPlans)
            {
                lessonPlans.Add(new LessonPlanResponse()
                {
                    ID = item.ID,
                    Slot = item.Slot,
                    Title = item.Title,
                });
            }

            return new SubjectResponse()
            {
                ID = subjectExist.ID,
                Name = subjectExist.Name,
                Grade = subjectExist.Grade,
                ComponentScores = componentScores.OrderBy(c => c.Semester).ThenBy(c => c.Name).ToList(),
                LessonPlans = lessonPlans.OrderBy(l => l.Slot).ToList()
            };
        }

        public async Task AddSubject(string accountID, SubjectRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Subject subjectExist = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(request.Name.ToLower().Trim())
                && s.Grade.ToLower().Equals(request.Grade.ToLower().Trim()) && s.IsActive);

            if (subjectExist != null)
            {
                throw new ArgumentException("Môn học đã tồn tại");
            }

            if (HasDuplicateSlot(request.LessonPlans))
            {
                throw new ArgumentException("Có tiết học bị trùng");
            }

            if (HasDuplicateNameAndSemester(request.ComponentScores))
            {
                throw new ArgumentException("Có điểm thành phần bị trùng");
            }

            Guid guid = Guid.NewGuid();

            Subject subject = new()
            {
                ID = guid,
                Name = request.Name,
                Grade = request.Grade,
                IsActive = true,
            };

            await _context.Subjects.AddAsync(subject);

            List<ComponentScore> componentScores = new();

            foreach (var item in request.ComponentScores)
            {
                componentScores.Add(new ComponentScore()
                {
                    ID = Guid.NewGuid(),
                    Count = item.Count,
                    Name = item.Name,
                    ScoreFactor = item.ScoreFactor,
                    Semester = item.Semester,
                    SubjectID = guid,
                });
            }
            
            await _context.ComponentScores.AddRangeAsync(componentScores);

            List<LessonPlans> lessonPlans = new();

            foreach (var item in request.LessonPlans)
            {
                lessonPlans.Add(new LessonPlans()
                {
                    ID = Guid.NewGuid(),
                    Slot = item.Slot,
                    Title = item.Title,
                    SubjectID = guid,
                });
            }

            await _context.LessonsPlans.AddRangeAsync(lessonPlans);
            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện thêm môn học " + request.Name + " khối " + request.Grade,
                Type = LogName.CREATE.ToString(),
            });
        }

        public async Task AddSubjectsByExcel(string accountID, ExcelRequest request)
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
                            var subjectRequest = new SubjectRequest
                            {
                                Name = worksheet.Cell("B1").GetString(),
                                Grade = worksheet.Cell("B2").GetString(),
                                ComponentScores = new List<ComponentScoreRequest>(),
                                LessonPlans = new List<LessonPlanRequest>()
                            };

                            int row = 5;
                            while (!worksheet.Cell(row, 1).GetString().Equals("Giáo án"))
                            {
                                var componentScore = new ComponentScoreRequest
                                {
                                    Name = worksheet.Cell(row, 1).GetString(),
                                    ScoreFactor = Decimal.Parse(worksheet.Cell(row, 2).GetString()),
                                    Count = int.Parse(worksheet.Cell(row, 3).GetString()),
                                    Semester = worksheet.Cell(row, 4).GetString()
                                };
                                subjectRequest.ComponentScores.Add(componentScore);
                                row++;
                            }

                            row += 2; 
                            while (!string.IsNullOrWhiteSpace(worksheet.Cell(row, 1).GetString()))
                            {
                                var lessonPlan = new LessonPlanRequest
                                {
                                    Slot = int.Parse(worksheet.Cell(row, 1).GetString()),
                                    Title = worksheet.Cell(row, 2).GetString()
                                };
                                subjectRequest.LessonPlans.Add(lessonPlan);
                                row++;
                            }

                            await AddSubject(accountID, subjectRequest);
                        }
                    }
                }
            }
        }

        public async Task UpdateSubject(string accountID, string subjectID, SubjectRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Subject subjectExist = await _context.Subjects
                        .Include(s => s.LessonPlans)
                        .Include(s => s.ComponentScores)
                        .FirstOrDefaultAsync(s => s.ID.ToString().ToLower().Equals(subjectID.ToLower().Trim()) && s.IsActive);

                    if (subjectExist == null)
                    {
                        throw new NotFoundException("Không tìm thấy môn học");
                    }

                    if (subjectExist.ComponentScores.Count > 0) _context.ComponentScores.RemoveRange(subjectExist.ComponentScores);
                    if (subjectExist.LessonPlans.Count > 0) _context.LessonsPlans.RemoveRange(subjectExist.LessonPlans);

                    Subject subjectName = await _context.Subjects
                        .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(request.Name.ToLower().Trim())
                        && s.Grade.ToLower().Equals(request.Grade.ToLower().Trim()) && s.IsActive);

                    if (subjectName != null)
                    {
                        throw new ArgumentException("Môn học đã tồn tại");
                    }

                    if (HasDuplicateSlot(request.LessonPlans))
                    {
                        throw new ArgumentException("Có tiết học bị trùng");
                    }

                    if (HasDuplicateNameAndSemester(request.ComponentScores))
                    {
                        throw new ArgumentException("Có điểm thành phần bị trùng");
                    }

                    Guid guid = new Guid(subjectID);

                    subjectExist.Name = request.Name;
                    subjectExist.Grade = request.Grade;

                    List<ComponentScore> componentScores = new();

                    foreach (var item in request.ComponentScores)
                    {
                        componentScores.Add(new ComponentScore()
                        {
                            ID = Guid.NewGuid(),
                            Count = item.Count,
                            Name = item.Name,
                            ScoreFactor = item.ScoreFactor,
                            Semester = item.Semester,
                            SubjectID = guid,
                        });
                    }

                    await _context.ComponentScores.AddRangeAsync(componentScores);

                    List<LessonPlans> lessonPlans = new();

                    foreach (var item in request.LessonPlans)
                    {
                        lessonPlans.Add(new LessonPlans()
                        {
                            ID = Guid.NewGuid(),
                            Slot = item.Slot,
                            Title = item.Title,
                            SubjectID = guid,
                        });
                    }

                    await _context.LessonsPlans.AddRangeAsync(lessonPlans);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
                    {
                        AccountID = accountID,
                        Note = "Người dùng " + account.Username + " vừa thực hiện chỉnh sửa môn học " + request.Name + " khối " + request.Grade,
                        Type = LogName.UPDATE.ToString(),
                    });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw; // Rethrow the exception so it can be handled outside this function
                }
            }
        }

        public async Task DeleteSubject(string accountID, string subjectID)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower()
                .Equals(accountID.ToLower())) ?? throw new ArgumentException("Tài khoản của bạn không tồn tại");

            Subject subjectExist = await _context.Subjects
                        .FirstOrDefaultAsync(s => s.ID.ToString().ToLower().Equals(subjectID.ToLower().Trim()) && s.IsActive);

            if (subjectExist == null)
            {
                throw new NotFoundException("Không tìm thấy môn học");
            }

            subjectExist.IsActive = false;

            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện xóa môn học " + subjectExist.Name + " khối " + subjectExist.Grade,
                Type = LogName.DELETE.ToString(),
            });
        }

        private bool HasDuplicateSlot(List<LessonPlanRequest> lessonPlanRequests)
        {
            var slots = new HashSet<int>();

            foreach (var request in lessonPlanRequests)
            {
                if (!slots.Add(request.Slot))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasDuplicateNameAndSemester(List<ComponentScoreRequest> componentScoreRequests)
        {
            var nameSemesterCombinations = new HashSet<string>();

            foreach (var request in componentScoreRequests)
            {
                string combination = request.Name + request.Semester;
                if (!nameSemesterCombinations.Add(combination))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
