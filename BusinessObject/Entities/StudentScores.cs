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

        public Guid ComponentScoreID { get; set; }

        [MaxLength(50)]
        public string StudentID { get; set; }

        public Guid SchoolYearID { get; set; }

        [MaxLength(10)]
        public string Score { get; set; }

        [Required]
        [MaxLength(50)]
        public string CreateBy { get; set; }

        [Required]
        [MaxLength(50)]
        public string UpdateBy { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public DateTime UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("ComponentScoreID")]
        public virtual ComponentScore ComponentScore { get; set; }

        [ForeignKey("StudentID")]
        public virtual AccountStudent AccountStudent { get; set; }

        [ForeignKey("SchoolYearID")]
        public virtual SchoolYear SchoolYear { get; set; }
    }
}
