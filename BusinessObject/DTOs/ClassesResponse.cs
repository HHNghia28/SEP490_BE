using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClassesResponse
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Teacher { get; set; }

        public string Classroom { get; set; }

        [Required]
        public string SchoolYear { get; set; }
    }
}
