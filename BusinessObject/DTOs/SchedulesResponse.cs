using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SchedulesResponse
    {
        public string SchoolYear { get; set; }
        public string Class { get; set; }
        public string MainTeacher { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Semester { get; set; } = "";
        public List<ScheduleDetailResponse> Details { get; set; }
    }
}
