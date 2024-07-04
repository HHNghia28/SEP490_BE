using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoreSubjectWithSemesterResponse
    {
        public string Subject { get; set; }
        public string Semester1Average { get; set; }
        public string Semester2Average { get; set; }
        public string YearAverage { get; set; }
    }
}
