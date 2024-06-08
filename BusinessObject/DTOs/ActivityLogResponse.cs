using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ActivityLogResponse
    {
        public Guid ID { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string Date { get; set; }
    }
}
