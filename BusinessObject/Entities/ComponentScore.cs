using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class ComponentScore
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public decimal ScoreFactor { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        [MaxLength(250)]
        public string Semester { get; set; }

        [Required]
        public Guid SubjectID { get; set; }

        // Navigation property
        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; }
    }
}
