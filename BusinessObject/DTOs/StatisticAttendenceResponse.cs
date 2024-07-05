using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class StatisticAttendenceResponse
    {
        public string ClassName { get; set; }
        public string Grade { get; set; }
        public string Teacher { get; set; }
        public int NumberOfStudent {  get; set; }
        public int NumberOfPresent {  get; set; }
        public int NumberOfAbsent {  get; set; }
        public int NumberOfNotYet {  get; set; }
    }
}
