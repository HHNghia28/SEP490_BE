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
        public Task AddSchedule(string accountID, ScheduleRequest request);
        public Task AddScheduleByExcel(string accountID, ScheduleExcelRequest request);
        public Task<ScheduleResponse> GetSchedulesByStudent(string studentID, string fromDate, string schoolYear);
        public Task<ScheduleResponse> GetSchedulesBySubjectTeacher(string teacherID, string fromDate, string schoolYear);
        public Task<ScheduleResponse> GetSchedulesByHomeroomTeacher(string teacherID, string classname, string fromDate, string schoolYear);
        public Task DeleteSchedule(string accountID, string scheduleID);
    }
}
