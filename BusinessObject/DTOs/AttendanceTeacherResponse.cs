using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AttendanceTeacherResponse
    {
        public string ScheduleID { get; set; }
        public string TeacherID { get; set; }
        public string TeacherName { get; set; }
        public string Avatar { get; set; }
        public string Classname { get; set; }
        public bool Present { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public int Slot { get; set; }
    }
}
