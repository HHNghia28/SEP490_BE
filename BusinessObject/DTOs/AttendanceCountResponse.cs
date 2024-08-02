using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AttendanceCountResponse
    {
        public string StudentId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Avatar {  get; set; }
        public int NumberOfSlot { get; set; }
        public int NumberOfPresent {  get; set; }
        public int NumberOfAbsent { get; set; }
        public int NumberOfConfirm { get; set; }
    }
}
