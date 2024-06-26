using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AverageScoresResponse
    {
        public string Class { get; set; }
        public string SchoolYear { get; set; }
        public string TeacherName { get; set; }
        public List<AverageScoreResponse> Averages { get; set; }
    }
}
