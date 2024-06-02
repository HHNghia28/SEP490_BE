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
    }
}
