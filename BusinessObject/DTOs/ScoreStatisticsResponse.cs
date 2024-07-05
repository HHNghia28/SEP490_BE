using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScoreStatisticsResponse
    {
        public int Grade { get; set; }
        public List<SemesterScore> Semesters { get; set; }
    }

    public class SemesterScore
    {
        public string Name { get; set; }
        public List<NameScore> Scores { get; set; }

    }

    public class NameScore
    {
        public string Name { get; set; }
        public List<CountScore> ScoreCounts { get; set; }
    }

    public class CountScore
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ScoreGradeStatisticsResponse
    {
        public string AverageScore { get; set; }
        public int Count { get; set; }
    }

    public class ScoreAverageStatisticsResponse
    {
        public string Semester { get; set; }
        public List<ScoreGradeStatisticsResponse> Scores { get; set; }
    }

}
