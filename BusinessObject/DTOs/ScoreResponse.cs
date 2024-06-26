using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoreResponse
    {
        public string ID { get; set; }
        public string FullName { get; set; }
        public double AverageYear { get; set; } 
        public double AverageSemester1 { get; set; } 
        public double AverageSemester2 { get; set; } 
        public int Rank { get; set; }
        public List<ScoreDetailResponse> Scores { get; set; }
    }
}
