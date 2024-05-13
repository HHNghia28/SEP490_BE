using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IActivityLogRepository
    {
        public Task WriteLogAsync(ActivityLogRequest request);
        public Task<IEnumerable<ActivityLogResponse>> GetLog();
    }
}
