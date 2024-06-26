using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SettingRequest
    {
        [Required]
        [MaxLength(100)]
        public string SchoolName { get; set; }

        [Required]
        [MaxLength(250)]
        public string SchoolAddress { get; set; }

        [Required]
        [MaxLength(10)]
        public string SchoolPhone { get; set; }

        [Required]
        [MaxLength(100)]
        public string SchoolEmail { get; set; }

        [Required]
        [MaxLength(50)]
        public string SchoolLevel { get; set; }
    }
}
