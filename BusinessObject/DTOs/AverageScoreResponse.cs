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
        public List<SubjectAverageResponse> SubjectAverages { get; set; }
    }
}
