using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoreStudentResponse
    {
        public string FullName { get; set; }
        public string SchoolYear { get; set; }
        public string ClassName { get; set; }
        public List<ScoreSubjectResponse> Details { get; set; }
    }
}
