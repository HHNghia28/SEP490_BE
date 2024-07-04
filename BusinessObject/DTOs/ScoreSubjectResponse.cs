using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoreSubjectResponse
    {
        public string Subject { get; set; }
        public string Average { get; set; }
        public List<ScoreDetailResponse> Scores { get; set; }
    }
}
