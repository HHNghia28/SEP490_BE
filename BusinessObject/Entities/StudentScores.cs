using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class StudentScores
    {
        [Key]
        public Guid ID { get; set; }

        [MaxLength(50)]
        public string StudentID { get; set; }

        public Guid SchoolYearID { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public decimal ScoreFactor { get; set; }

        [Required]
        [MaxLength(250)]
        public string Semester { get; set; }

        [MaxLength(10)]
        public string Score { get; set; }

        [ForeignKey("StudentID")]
        public virtual AccountStudent AccountStudent { get; set; }

        [ForeignKey("SchoolYearID")]
        public virtual SchoolYear SchoolYear { get; set; }
    }
}
