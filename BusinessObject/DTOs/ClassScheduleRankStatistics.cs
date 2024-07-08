using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClassScheduleRankStatistics
    {
        public string ClassName { get; set; }
        public Dictionary<string, int> RankCounts { get; set; }
    }
}
