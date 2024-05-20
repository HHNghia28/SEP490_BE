using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScheduleResponse
    {
        public string ID { get; set; }
        public int Slot {  get; set; }
        public string Classroom { get; set; }
        public string Classname { get; set; }
        public string SlotTime { get; set; }
        public string Subject { get; set; }
        public int SlotByLessonPlans { get; set; }
        public string Teacher { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public bool IsAttendance {  get; set; } 
    }
}
