using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AttendenceResponse
    {
        public string AttendenceID { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Avatar {  get; set; }
        public bool Present { get; set; }
        public string Date {  get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Teacher { get; set; }
        public int Slot { get; set; }
    }
}
