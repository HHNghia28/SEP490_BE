using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClassStudentResponse
    {
        public string ID { get; set; }

        public string Fullname { get; set; }

        public string Gender { get; set; }
        public string? Avatar { get; set; }
    }
}
