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
        public Task<ScheduleResponse> GetSchedulesByStudents(string studentID, int currentIndex, string schoolYear);
    }
}
