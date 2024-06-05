using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoresResponse
    {
        public string Subject { get; set; }
        public string Class { get; set; }
        public string SchoolYear { get; set; }
        public string TeacherName { get; set; }
        public List<ScoreResponse> Score {  get; set; }
    }
}
