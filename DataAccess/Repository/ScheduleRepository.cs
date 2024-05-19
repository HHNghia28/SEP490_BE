using BusinessObject.DTOs;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using ClosedXML.Excel;
using DataAccess.Context;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogRepository _activityLogRepository;

        public ScheduleRepository(ApplicationDbContext context, IActivityLogRepository activityLogRepository)
        {
            _context = context;
            _activityLogRepository = activityLogRepository;
        }

        public async Task AddScheduleByExcel(string accountID, ScheduleExcelRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(accountID.ToLower())) ?? throw new NotFoundException("Tài khoản của bạn không tồn tại");

            IFormFile file = request.ScheduleFile;
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
                            List<Schedule> schedules = new();
                            List<Attendance> attendances = new();
                            List<ScheduleSubject> scheduleSubjectsS1 = new();
                            List<ScheduleDaily> scheduleDailiesS1 = new();
                            List<ScheduleSubject> scheduleSubjectsS2 = new();
                            List<ScheduleDaily> scheduleDailiesS2 = new();
                            Account teacher = new();
                            Subject subject = new();
                            Classes studentClass = new();
                            string classes = "";
                            string schoolYear = "";
                            DateTime fromDateS1 = DateTime.Now;
                            DateTime fromDateS2 = DateTime.Now;
                            bool IsSemester2 = false;
                            int currentIndex = 0;
                            Dictionary<string, int> subjectS2 = new();

                            List<string> data = new();

                            for (int row = 1; row <= worksheet.LastRowUsed().RowNumber(); row++)
                            {
                                for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                                {
                                    var cell = worksheet.Cell(row, col);
                                    data.Add(cell.Value.ToString());
                                }
                            }

                            if (!IsSemester2)
                            {
                                for (int i = 0; i < data.Count; i++)
                                {
                                    switch (data[i])
                                    {
                                        case "Lớp":
                                            i++;
                                            classes = data[i];
                                            break;
                                        case "Năm học":
                                            i++;
                                            schoolYear = data[i];
                                            break;
                                        case "Ngày bắt đầu":
                                            try
                                            {
                                                i++;
                                                fromDateS1 = DateTime.ParseExact(data[i], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            }
                                            catch (FormatException)
                                            {
                                                throw new ArgumentException("Ngày bắt đầu không đúng định dạng. Vui lòng sử dụng 'dd/MM/yyyy'.");
                                            }
                                            break;
                                        case "Môn học":
                                            i += 3;
                                            int count = 0;
                                            string str;
                                            ScheduleSubject scheduleSubject = new();
                                            for (int j = 0; j < data.Count; j++)
                                            {
                                                i++;
                                                str = data[i];
                                                if (string.IsNullOrEmpty(str))
                                                {
                                                    continue;
                                                }

                                                if (str.Equals("Thời khóa biểu"))
                                                {
                                                    i--;
                                                    break;
                                                }

                                                switch (count)
                                                {
                                                    case 0:
                                                        count++;
                                                        subject = await _context.Subjects
                                                            .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(str.ToLower())) ?? throw new NotFoundException("Không tìm thấy môn học " + str);
                                                        scheduleSubject.SubjectID = subject.ID;
                                                        scheduleSubject.Subject = subject.Name;
                                                        break;
                                                    case 1:
                                                        count++;
                                                        scheduleSubject.Count = int.Parse(str);
                                                        break;
                                                    case 2:
                                                        count++;
                                                        teacher = await _context.Accounts
                                                            .FirstOrDefaultAsync(a => a.Username.ToLower().Equals(str.ToLower())) ?? throw new NotFoundException("Không tìm thấy giáo viên " + str);
                                                        scheduleSubject.TeacherID = teacher.ID;
                                                        break;
                                                }

                                                if (count == 3)
                                                {
                                                    count = 0;
                                                    scheduleSubjectsS1.Add(scheduleSubject);
                                                    scheduleSubject = new();
                                                }
                                            }
                                            break;
                                        case "Thời khóa biểu":
                                            i += 6;
                                            int countSchedule = 0;
                                            string strSchedule;
                                            ScheduleDaily scheduleDaily = new();
                                            for (int j = 0; j < data.Count; j++)
                                            {
                                                i++;
                                                if (i == data.Count) break;
                                                strSchedule = data[i];

                                                if (string.IsNullOrEmpty(strSchedule))
                                                {
                                                    continue;
                                                }

                                                if (strSchedule.ToLower().Equals("Học kì 2".ToLower()))
                                                {
                                                    IsSemester2 = true;
                                                    break;
                                                }

                                                switch (strSchedule)
                                                {
                                                    case "1":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 1,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "2":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 2,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "3":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 3,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "4":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 4,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "5":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 5,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "6":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 6,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "7":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 7,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "8":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 8,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "9":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 9,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "10":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 10,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS1.Add(scheduleDaily);
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                    currentIndex = i;
                                    if (IsSemester2) break;
                                }

                                Guid schoolYearID = Guid.NewGuid();
                                SchoolYear schoolY = await _context.SchoolYears
                                    .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(schoolYear.ToLower()));

                                if (schoolY == null)
                                {
                                    await _context.SchoolYears.AddAsync(new SchoolYear()
                                    {
                                        ID = schoolYearID,
                                        Name = schoolYear,
                                    });
                                }
                                else
                                {
                                    schoolYearID = schoolY.ID;
                                }

                                foreach (var item in scheduleSubjectsS1)
                                {
                                    subjectS2.Add(item.Subject, item.Count);
                                }

                                string json = JsonSerializer.Serialize(scheduleSubjectsS1);

                                List<ScheduleSubject> copiedList = JsonSerializer.Deserialize<List<ScheduleSubject>>(json);

                                studentClass = await _context.Classes
                                    .Include(c => c.SchoolYear)
                                    .Include(c => c.StudentClasses)
                                    .ThenInclude(c => c.AccountStudent)
                                    .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(classes.ToLower())
                                    && Guid.Equals(schoolYearID, c.SchoolYearID)) ?? throw new NotFoundException("Không tìm thấy lớp học " + classes + " ở năm học " + schoolYear);

                                for (int i = 0; i < 100; i++)
                                {
                                    List<DateTime> weekDates = GetDatesToNextSunday(fromDateS1);

                                    foreach (var item in weekDates)
                                    {
                                        foreach (var daily in scheduleDailiesS1)
                                        {
                                            if ((int)item.DayOfWeek == daily.WeekDate)
                                            {
                                                ScheduleSubject subjectSche = scheduleSubjectsS1.FirstOrDefault(s => s.Subject.ToLower().Equals(daily.Subject.ToLower())) ?? throw new NotFoundException("Thiếu số lượng môn học " + daily.Subject);
                                                ScheduleSubject subjectCopy = copiedList.FirstOrDefault(s => s.Subject.ToLower().Equals(daily.Subject.ToLower())) ?? throw new NotFoundException("Thiếu số lượng môn học " + daily.Subject);

                                                if (subjectSche.Count <= 0)
                                                {
                                                    continue;
                                                }

                                                schedules.Add(new()
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ClassID = studentClass.ID,
                                                    Date = item,
                                                    Note = "",
                                                    Rank = "",
                                                    SlotByDate = daily.Slot,
                                                    SubjectID = subjectSche.SubjectID,
                                                    TeacherID = subjectSche.TeacherID,
                                                    SlotByLessonPlans = subjectCopy.Count - subjectSche.Count + 1
                                                });
                                                subjectSche.Count--;
                                            }
                                        }
                                    }

                                    fromDateS1 = weekDates.ElementAt(weekDates.Count - 1);
                                }

                            }

                            if (IsSemester2)
                            {
                                for (int i = currentIndex; i < data.Count; i++)
                                {
                                    switch (data[i])
                                    {
                                        case "Lớp":
                                            i++;
                                            classes = data[i];
                                            break;
                                        case "Năm học":
                                            i++;
                                            schoolYear = data[i];
                                            break;
                                        case "Ngày bắt đầu":
                                            try
                                            {
                                                i++;
                                                fromDateS2 = DateTime.ParseExact(data[i], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            }
                                            catch (FormatException)
                                            {
                                                throw new ArgumentException("Ngày bắt đầu không đúng định dạng. Vui lòng sử dụng 'dd/MM/yyyy'.");
                                            }
                                            break;
                                        case "Môn học":
                                            i += 3;
                                            int count = 0;
                                            string str;
                                            ScheduleSubject scheduleSubject = new();
                                            for (int j = 0; j < data.Count; j++)
                                            {
                                                i++;
                                                str = data[i];
                                                if (string.IsNullOrEmpty(str))
                                                {
                                                    continue;
                                                }

                                                if (str.Equals("Thời khóa biểu"))
                                                {
                                                    i--;
                                                    break;
                                                }

                                                switch (count)
                                                {
                                                    case 0:
                                                        count++;
                                                        subject = await _context.Subjects
                                                            .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(str.ToLower())) ?? throw new NotFoundException("Không tìm thấy môn học " + str);
                                                        scheduleSubject.SubjectID = subject.ID;
                                                        scheduleSubject.Subject = subject.Name;
                                                        break;
                                                    case 1:
                                                        count++;
                                                        scheduleSubject.Count = int.Parse(str);
                                                        break;
                                                    case 2:
                                                        count++;
                                                        teacher = await _context.Accounts
                                                            .FirstOrDefaultAsync(a => a.Username.ToLower().Equals(str.ToLower())) ?? throw new NotFoundException("Không tìm thấy giáo viên " + str);
                                                        scheduleSubject.TeacherID = teacher.ID;
                                                        break;
                                                }

                                                if (count == 3)
                                                {
                                                    count = 0;
                                                    scheduleSubjectsS2.Add(scheduleSubject);
                                                    scheduleSubject = new();
                                                }
                                            }
                                            break;
                                        case "Thời khóa biểu":
                                            i += 6;
                                            int countSchedule = 0;
                                            string strSchedule;
                                            ScheduleDaily scheduleDaily = new();
                                            for (int j = 0; j < data.Count; j++)
                                            {
                                                i++;
                                                if (i == data.Count) break;
                                                strSchedule = data[i];

                                                if (string.IsNullOrEmpty(strSchedule))
                                                {
                                                    continue;
                                                }

                                                switch (strSchedule)
                                                {
                                                    case "1":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 1,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "2":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 2,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "3":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 3,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "4":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 4,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "5":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 5,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "6":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 6,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "7":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 7,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "8":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 8,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "9":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 9,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                    case "10":
                                                        for (int k = 0; k < 7; k++)
                                                        {
                                                            i++;
                                                            strSchedule = data[i];

                                                            if (string.IsNullOrEmpty(strSchedule))
                                                            {
                                                                continue;
                                                            }

                                                            scheduleDaily = new()
                                                            {
                                                                Slot = 10,
                                                                WeekDate = k,
                                                                Subject = strSchedule
                                                            };

                                                            scheduleDailiesS2.Add(scheduleDaily);
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }

                                Guid schoolYearID = Guid.NewGuid();
                                SchoolYear schoolY = await _context.SchoolYears
                                    .FirstOrDefaultAsync(s => s.Name.ToLower().Equals(schoolYear.ToLower()));

                                if (schoolY == null)
                                {
                                    await _context.SchoolYears.AddAsync(new SchoolYear()
                                    {
                                        ID = schoolYearID,
                                        Name = schoolYear,
                                    });
                                }
                                else
                                {
                                    schoolYearID = schoolY.ID;
                                }

                                string json = JsonSerializer.Serialize(scheduleSubjectsS2);

                                List<ScheduleSubject> copiedList = JsonSerializer.Deserialize<List<ScheduleSubject>>(json);

                                studentClass = await _context.Classes
                                    .Include(c => c.SchoolYear)
                                    .FirstOrDefaultAsync(c => c.Classroom.ToLower().Equals(classes.ToLower())
                                    && Guid.Equals(schoolYearID, c.SchoolYearID)) ?? throw new NotFoundException("Không tìm thấy lớp học " + classes + " ở năm học " + schoolYear);

                                for (int i = 0; i < 100; i++)
                                {
                                    List<DateTime> weekDates = GetDatesToNextSunday(fromDateS2);

                                    foreach (var item in weekDates)
                                    {
                                        foreach (var daily in scheduleDailiesS2)
                                        {
                                            if ((int)item.DayOfWeek == daily.WeekDate)
                                            {
                                                ScheduleSubject subjectSche = scheduleSubjectsS2.FirstOrDefault(s => s.Subject.ToLower().Equals(daily.Subject.ToLower())) ?? throw new NotFoundException("Thiếu số lượng môn học " + daily.Subject);
                                                ScheduleSubject subjectCopy = copiedList.FirstOrDefault(s => s.Subject.ToLower().Equals(daily.Subject.ToLower())) ?? throw new NotFoundException("Thiếu số lượng môn học " + daily.Subject);

                                                if (subjectSche.Count <= 0)
                                                {
                                                    continue;
                                                }

                                                schedules.Add(new()
                                                {
                                                    ID = Guid.NewGuid(),
                                                    ClassID = studentClass.ID,
                                                    Date = item,
                                                    Note = "",
                                                    Rank = "",
                                                    SlotByDate = daily.Slot,
                                                    SubjectID = subjectSche.SubjectID,
                                                    TeacherID = subjectSche.TeacherID,
                                                    SlotByLessonPlans = subjectCopy.Count - subjectSche.Count + 1 + subjectS2[subjectSche.Subject]
                                                });
                                                subjectSche.Count--;
                                            }
                                        }
                                    }

                                    fromDateS2 = weekDates.ElementAt(weekDates.Count - 1);
                                }
                            }

                            List<Schedule> schedulesExist = await _context.Schedules
                            .Where(s => Guid.Equals(s.ClassID, studentClass.ID)).ToListAsync();

                            List<Attendance> attendancesExist = await _context.Attendances
                                .Where(a => schedulesExist.Select(item => item.ID).Contains(a.ScheduleID)).ToListAsync();

                            if (schedulesExist.Count > 0)
                            {
                                _context.Schedules.RemoveRange(schedulesExist);
                                _context.Attendances.RemoveRange(attendancesExist);
                            }

                            await _context.Schedules.AddRangeAsync(schedules);

                            foreach (var item in schedules)
                            {
                                foreach (var item1 in studentClass.StudentClasses)
                                {
                                    attendances.Add(new()
                                    {
                                        ID = Guid.NewGuid(),
                                        StudentID = item1.AccountStudent.ID,
                                        ScheduleID = item.ID
                                    });
                                }
                            }

                            await _context.Attendances.AddRangeAsync(attendances);

                            await _context.SaveChangesAsync();

                            if (schedulesExist.Count > 0)
                            {
                                await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
                                {
                                    AccountID = accountID,
                                    Note = "Người dùng " + account.Username + " vừa thực hiện cập nhật thời khóa biểu lớp học " + classes + " của năm học " + schoolYear,
                                    Type = LogName.UPDATE.ToString(),
                                });
                            }
                            else
                            {
                                await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
                                {
                                    AccountID = accountID,
                                    Note = "Người dùng " + account.Username + " vừa thực hiện thêm thời khóa biểu lớp học " + classes + " của năm học " + schoolYear,
                                    Type = LogName.CREATE.ToString(),
                                });
                            }
                        }
                    }
                }
            }
        }

        public async Task<ScheduleResponse> GetSchedulesByStudent(string studentID, string fromDate, string schoolYear)
        {
            Classes classes = await _context.Classes
                .AsNoTracking()
                .Include(c => c.Teacher)
                .Include(c => c.SchoolYear)
                .Include(c => c.StudentClasses)
                .ThenInclude(c => c.AccountStudent)
                .Where(c => c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower()))
                .FirstOrDefaultAsync(c => c.IsActive && c.StudentClasses
                .Select(item => item.StudentID.ToLower()).Contains(studentID.ToLower())) ?? throw new NotFoundException("Không tìm thấy lớp học");

            List<DateTime> dates = GetDatesToNextSunday(DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .Where(s => Guid.Equals(s.ClassID, classes.ID) && dates.Contains(s.Date))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.SlotByDate)
                .ToListAsync();

            ScheduleResponse schedulesResponse = new()
            {
                Class = classes.Classroom,
                FromDate = dates.ElementAt(0).ToString("dd/MM/yyyy"),
                ToDate = dates.ElementAt(dates.Count - 1).ToString("dd/MM/yyyy"),
                MainTeacher = classes.Teacher.Username,
                SchoolYear = schoolYear
            };

            List<ScheduleDetailResponse> scheduleDetailResponse = new();

            foreach (var item in dates)
            {
                List<ScheduleSlotResponse> slotResponses = new();

                for (int i = 1; i < 11; i++)
                {
                    Schedule schedule = schedules.FirstOrDefault(s => s.SlotByDate == i && s.Date == item);

                    if (schedule == null)
                    {
                        slotResponses.Add(new ScheduleSlotResponse()
                        {
                            ID = Guid.NewGuid().ToString(),
                            Slot = i,
                            Classroom = "",
                            SlotTime = GetSlotTime(i),
                            SlotByLessonPlans = 0,
                            Status = "",
                            IsAttendance = false,
                            Teacher = "",
                            Subject = ""
                        });
                    }
                    else
                    {
                        slotResponses.Add(new ScheduleSlotResponse()
                        {
                            ID = schedule.ID.ToString(),
                            Slot = schedule.SlotByDate,
                            Classroom = schedule.Subject.Name.Equals("Chào cờ") ? "Sân chào cờ" : "Phòng " + classes.Classroom,
                            SlotTime = GetSlotTime(i),
                            SlotByLessonPlans = schedule.SlotByLessonPlans,
                            Status = schedule.Date > DateTime.Now ? "Chưa bắt đầu" : schedule.Attendances.FirstOrDefault(a => a.StudentID.Equals(studentID)).Present ? "Có mặt" : "Vắng",
                            IsAttendance = schedule.Attendances.FirstOrDefault(a => a.StudentID.Equals(studentID)).Present,
                            Teacher = schedule.Teacher.Username,
                            Subject = schedule.Subject.Name
                        });
                    }
                }

                scheduleDetailResponse.Add(new ScheduleDetailResponse()
                {
                    ID = Guid.NewGuid().ToString(),
                    Date = item.ToString("dd/MM/yyyy"),
                    WeekDate = GetVietnameseDayOfWeek(item.DayOfWeek),
                    Slots = slotResponses,
                });
            }

            schedulesResponse.Details = scheduleDetailResponse;

            return schedulesResponse;
        }

        public async Task<ScheduleResponse> GetSchedulesBySubjectTeacher(string teacherID, string fromDate, string schoolYear)
        {
            Account teacher = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(teacherID.ToLower()) && a.IsActive) ?? throw new NotFoundException("Không tìm thấy tài khoản giáo viên");

            List<DateTime> dates = GetDatesToNextSunday(DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .Where(s => s.TeacherID.ToLower().Equals(teacherID.ToLower()) && dates.Contains(s.Date))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.SlotByDate)
                .ToListAsync();

            ScheduleResponse schedulesResponse = new()
            {
                Class = "",
                FromDate = dates.ElementAt(0).ToString("dd/MM/yyyy"),
                ToDate = dates.ElementAt(dates.Count - 1).ToString("dd/MM/yyyy"),
                MainTeacher = teacher.Username,
                SchoolYear = schoolYear
            };

            List<ScheduleDetailResponse> scheduleDetailResponse = new();

            foreach (var item in dates)
            {
                List<ScheduleSlotResponse> slotResponses = new();

                for (int i = 1; i < 11; i++)
                {
                    Schedule schedule = schedules.FirstOrDefault(s => s.SlotByDate == i && s.Date == item);

                    if (schedule == null)
                    {
                        slotResponses.Add(new ScheduleSlotResponse()
                        {
                            ID = Guid.NewGuid().ToString(),
                            Slot = i,
                            Classroom = "",
                            SlotTime = GetSlotTime(i),
                            SlotByLessonPlans = 0,
                            Status = "",
                            IsAttendance = false,
                            Teacher = "",
                            Subject = ""
                        });
                    }
                    else
                    {
                        slotResponses.Add(new ScheduleSlotResponse()
                        {
                            ID = schedule.ID.ToString(),
                            Slot = schedule.SlotByDate,
                            Classroom = schedule.Subject.Name.Equals("Chào cờ") ? "Sân chào cờ" : "Phòng " + schedule.Classes.Classroom,
                            SlotTime = GetSlotTime(i),
                            SlotByLessonPlans = schedule.SlotByLessonPlans,
                            Status = schedule.Date > DateTime.Now ? "Chưa bắt đầu" : !string.IsNullOrEmpty(schedule.Rank) ? "Có mặt" : "Vắng",
                            IsAttendance = !string.IsNullOrEmpty(schedule.Rank),
                            Teacher = schedule.Teacher.Username,
                            Subject = schedule.Subject.Name
                        });
                    }
                }

                scheduleDetailResponse.Add(new ScheduleDetailResponse()
                {
                    ID = Guid.NewGuid().ToString(),
                    Date = item.ToString("dd/MM/yyyy"),
                    WeekDate = GetVietnameseDayOfWeek(item.DayOfWeek),
                    Slots = slotResponses,
                });
            }

            schedulesResponse.Details = scheduleDetailResponse;

            return schedulesResponse;
        }

        public async Task<ScheduleResponse> GetSchedulesByHomeroomTeacher(string teacherID, string classname, string fromDate, string schoolYear)
        {
            Account teacher = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(teacherID.ToLower()) && a.IsActive) ?? throw new NotFoundException("Không tìm thấy tài khoản giáo viên");

            Classes classes = await _context.Classes
                .Include(c => c.SchoolYear)
                .FirstOrDefaultAsync(c => c.TeacherID.ToLower().Equals(teacherID.ToLower())
                && c.Classroom.ToLower().Equals(classname.ToLower()) 
                && c.SchoolYear.Name.ToLower().Equals(schoolYear.ToLower())) ?? throw new NotFoundException("Không tìm thấy lớp chủ nhiệm");

            List<DateTime> dates = GetDatesToNextSunday(DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classes)
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .Where(s => Guid.Equals(s.ClassID, classes.ID) && dates.Contains(s.Date))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.SlotByDate)
                .ToListAsync();

            ScheduleResponse schedulesResponse = new()
            {
                Class = "",
                FromDate = dates.ElementAt(0).ToString("dd/MM/yyyy"),
                ToDate = dates.ElementAt(dates.Count - 1).ToString("dd/MM/yyyy"),
                MainTeacher = teacher.Username,
                SchoolYear = schoolYear
            };

            List<ScheduleDetailResponse> scheduleDetailResponse = new();

            foreach (var item in dates)
            {
                List<ScheduleSlotResponse> slotResponses = new();

                for (int i = 1; i < 11; i++)
                {
                    Schedule schedule = schedules.FirstOrDefault(s => s.SlotByDate == i && s.Date == item);

                    if (schedule == null)
                    {
                        slotResponses.Add(new ScheduleSlotResponse()
                        {
                            ID = Guid.NewGuid().ToString(),
                            Slot = i,
                            Classroom = "",
                            SlotTime = GetSlotTime(i),
                            SlotByLessonPlans = 0,
                            Status = "",
                            IsAttendance = false,
                            Teacher = "",
                            Subject = ""
                        });
                    }
                    else
                    {
                        slotResponses.Add(new ScheduleSlotResponse()
                        {
                            ID = schedule.ID.ToString(),
                            Slot = schedule.SlotByDate,
                            Classroom = schedule.Subject.Name.Equals("Chào cờ") ? "Sân chào cờ" : "Phòng " + schedule.Classes.Classroom,
                            SlotTime = GetSlotTime(i),
                            SlotByLessonPlans = schedule.SlotByLessonPlans,
                            Status = schedule.Date > DateTime.Now ? "Chưa bắt đầu" : !string.IsNullOrEmpty(schedule.Rank) ? "Có mặt" : "Vắng",
                            IsAttendance = !string.IsNullOrEmpty(schedule.Rank),
                            Teacher = schedule.Teacher.Username,
                            Subject = schedule.Subject.Name
                        });
                    }
                }

                scheduleDetailResponse.Add(new ScheduleDetailResponse()
                {
                    ID = Guid.NewGuid().ToString(),
                    Date = item.ToString("dd/MM/yyyy"),
                    WeekDate = GetVietnameseDayOfWeek(item.DayOfWeek),
                    Slots = slotResponses,
                });
            }

            schedulesResponse.Details = scheduleDetailResponse;

            return schedulesResponse;
        }

        public async Task DeleteSchedule(string accountID, string scheduleID)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(accountID.ToLower())) ?? throw new NotFoundException("Tài khoản của bạn không tồn tại");

            Schedule schedule = await _context.Schedules
                .Include(s => s.Classes)
                .ThenInclude(s => s.SchoolYear)
                .FirstOrDefaultAsync(s => Guid.Equals(s.ID, new Guid(scheduleID))) ?? throw new NotFoundException("Không tìm thấy lịch học");

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện xóa thời khóa biểu tiết " + schedule.SlotByDate + " " + GetVietnameseDayOfWeek(schedule.Date.DayOfWeek)
                + " của lớp học " + schedule.Classes.Classroom + " năm học " + schedule.Classes.SchoolYear.Name,
                Type = LogName.DELETE.ToString(),
            });
        }

        public async Task AddSchedule(string accountID, ScheduleRequest request)
        {
            Account account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(accountID.ToLower())) ?? throw new NotFoundException("Tài khoản của bạn không tồn tại");

            Account teacher = await _context.Accounts
                .FirstOrDefaultAsync(a => a.ID.ToLower().Equals(request.TeacherID.ToLower())) ?? throw new NotFoundException("Giáo viên bộ môn không tồn tại");

            Subject subject = await _context.Subjects
                .FirstOrDefaultAsync(s => Guid.Equals(s.ID, request.SubjectID)) ?? throw new NotFoundException("Môn học không tồn tại");

            Classes classes = await _context.Classes
                .Include(c => c.SchoolYear)
                .FirstOrDefaultAsync(c => Guid.Equals(c.ID, request.ClassID)) ?? throw new NotFoundException("Lớp học không tồn tại");

            Schedule scheduleExist = await _context.Schedules
                .FirstOrDefaultAsync(s => s.TeacherID.ToLower().Equals(request.TeacherID.ToLower())
                && Guid.Equals(s.ClassID, request.ClassID)
                && s.SlotByDate == request.SlotByDate);

            if (scheduleExist != null)
            {
                throw new ArgumentException("Tiết học đã tồn tại");
            }

            Schedule schedule = new()
            {
                ID = Guid.NewGuid(),
                ClassID = request.ClassID,
                SubjectID = request.SubjectID,
                TeacherID = request.TeacherID,
                SlotByDate = request.SlotByDate,
                SlotByLessonPlans = request.SlotByLessonPlans,
                Date = request.Date,
                Note = "",
                Rank = ""
            };

            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            await _activityLogRepository.WriteLogAsync(new ActivityLogRequest()
            {
                AccountID = accountID,
                Note = "Người dùng " + account.Username + " vừa thực hiện xóa thời khóa biểu tiết " + request.SlotByDate + " " + GetVietnameseDayOfWeek(request.Date.DayOfWeek)
                + " của lớp học " + classes.Classroom + " năm học " + classes.SchoolYear.Name,
                Type = LogName.DELETE.ToString(),
            });
        }

        private List<DateTime> GetDatesToNextSunday(DateTime startDate)
        {
            List<DateTime> dates = new List<DateTime>();

            DateTime currentDate = startDate;

            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                currentDate = currentDate.AddDays(1);
            }

            while (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                dates.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            dates.Add(currentDate);

            return dates;
        }

        private string GetVietnameseDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Thứ Hai";
                case DayOfWeek.Tuesday:
                    return "Thứ Ba";
                case DayOfWeek.Wednesday:
                    return "Thứ Tư";
                case DayOfWeek.Thursday:
                    return "Thứ Năm";
                case DayOfWeek.Friday:
                    return "Thứ Sáu";
                case DayOfWeek.Saturday:
                    return "Thứ Bảy";
                case DayOfWeek.Sunday:
                    return "Chủ Nhật";
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Ngày không hợp lệ");
            }
        }

        private string GetSlotTime(int i)
        {
            switch (i)
            {
                case 1:
                    return "7h10-7h55";
                case 2:
                    return "8h00-8h45";
                case 3:
                    return "9h05-9h50";
                case 4:
                    return "9h55-10h40";
                case 5:
                    return "10h50-11h35";
                case 6:
                    return "12h45-13h20";
                case 7:
                    return "13h25-14h10";
                case 8:
                    return "14h30-15h15";
                case 9:
                    return "15h20-16h05";
                case 10:
                    return "16h15-17h00";
            }

            return "";
        }

        private class ScheduleSubject
        {
            public Guid SubjectID { get; set; }
            public string Subject { get; set; }
            public string TeacherID { get; set; }
            public int Count { get; set; }
        }

        private class ScheduleDaily
        {
            public string Subject { get; set; }
            public int WeekDate { get; set; }
            public int Slot { get; set; }
        }
    }
}
