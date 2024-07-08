using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IStatisticRepository
    {
        public Task<IEnumerable<StatisticAttendenceResponse>> GetStatisticAttendance(string schoolYear, int grade = 0, string fromDate = null, string toDate = null);
        public Task<IEnumerable<ScoreStatisticsResponse>> GetScoreStatistics(string schoolYear, string className = null, int grade = 0, string subject = null);
        public Task<IEnumerable<ScoreAverageStatisticsResponse>> GetScoreAverageStatistics(string schoolYear, string className = null, int grade = 0);
        public Task<IEnumerable<ScoreAverageStatisticsResponse>> GetGroupScoreAverageStatistics(string schoolYear, string className = null, int grade = 0);
        public Task<List<ClassScheduleRankStatistics>> GetScheduleRankCountBySchoolYearAsync(string schoolYear, string className = null, int grade = 0);
    }
}
