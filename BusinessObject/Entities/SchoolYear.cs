using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class SchoolYear
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ToDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string CreateBy { get; set; }

        [Required]
        [MaxLength(50)]
        public string UpdateBy { get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
