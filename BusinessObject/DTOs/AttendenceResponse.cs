using System;
using System.Collections.Generic;
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
    }
}
