using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SubjectAverageResponse
    {
        public string Subject { get; set; }
        public double AverageWholeYear { get; set; }
        public double AverageSemester1 { get; set; }
        public double AverageSemester2 { get; set; }
    }
}
