using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RegistersBookResponse
    {
        public string SchoolYear { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Classname { get; set; }
        public List<RegistersBookDetailResponse> Details { get; set; }
    }
}
