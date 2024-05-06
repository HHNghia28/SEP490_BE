using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SchoolYearRequest
    {
        [Column(TypeName = "date")]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ToDate { get; set; }
    }
}
