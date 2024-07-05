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
    }
}
