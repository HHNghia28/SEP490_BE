using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IScoreRepository
    {
        public Task<byte[]> GenerateExcelFile(string className, string schoolYear, string semester, string subject, string component, int indexCol = 1);
        public Task AddScoreByExcel(string accountID, ExcelRequest request);
        public Task UpdateScoreByExcel(string accountID, ExcelRequest request);
        public Task<ScoresResponse> GetScoresByClassBySubject(string className, string subjectName, string schoolYear);
        Task<AverageScoresResponse> GetAverageScoresByClass(string className, string schoolYear);
        public Task<ScoreStudentResponse> GetScoresByStudentAllSubject(string studentID, string schoolYear);
        public Task<ScoreStudentResponse> GetScoresByStudentBySubject(string studentID, string subject, string schoolYear);
    }
}
