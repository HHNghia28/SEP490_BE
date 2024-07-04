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
        public string AverageYear { get; set; } 
        public string AverageSemester1 { get; set; } 
        public string AverageSemester2 { get; set; } 
        public int Rank { get; set; }
        public List<ScoreDetailResponse> Scores { get; set; }
    }
}
