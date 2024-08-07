using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AverageScoreResponse
    {
        public string ID { get; set; }
        public string FullName { get; set; }
        public string TotalAverageWholeYear { get; set; }
        public string TotalAverageSemester1 { get; set; }
        public string TotalAverageSemester2 { get; set; }
        public string PerformanceWholeYear { get; set; }
        public string PerformanceSemester1 { get; set; }
        public string PerformanceSemester2 { get; set; }
        public int RankWholeYear { get; set; }
        public int RankSemester1 { get; set; }
        public int RankSemester2 { get; set; }
        public List<SubjectAverageResponse> SubjectAverages { get; set; }
    }
}
