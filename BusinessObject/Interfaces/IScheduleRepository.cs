using BusinessObject.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IScheduleRepository
    {
        public Task<ScheduleResponse> GetScheduleTeacher(string scheduleID);
        public Task<ScheduleResponse> GetScheduleStudent(string studentID, string scheduleID);
        public Task AddSchedule(string accountID, ScheduleRequest request);
        public Task UpdateSchedule(string accountID, string scheduleID, ScheduleRequest request);
        public Task AddScheduleByExcel(string accountID, ExcelRequest request);
        public Task<SchedulesResponse> GetSchedulesByStudent(string studentID, string fromDate, string schoolYear);
        public Task<SchedulesResponse> GetSchedulesByClass(string className, string fromDate, string schoolYear);
        public Task<SchedulesResponse> GetSchedulesBySubjectTeacher(string teacherID, string fromDate, string schoolYear);
        public Task<SchedulesResponse> GetSchedulesByHomeroomTeacher(string teacherID, string classname, string fromDate, string schoolYear);
        public Task DeleteSchedule(string accountID, string scheduleID);
    }
}
