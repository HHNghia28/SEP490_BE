using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ActivityLogRequest
    {
        public Guid ID { get; set; }    
        [ForeignKey("Account")]
        public string AccountID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        [MaxLength(250)]
        public string Note { get; set; }
    }
}
