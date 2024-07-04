using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoreDetailResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int IndexCol { get; set; }
        public string Semester { get; set; }
    }
}
