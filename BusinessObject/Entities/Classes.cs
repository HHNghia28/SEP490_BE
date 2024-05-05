using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Classes
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string TeacherID { get; set; }

        [Required]
        public Guid SchoolYearID { get; set; }

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

        // Navigation properties
        public virtual Account Teacher { get; set; }
        public virtual SchoolYear SchoolYear { get; set; }
        public virtual ICollection<StudentClasses> StudentClasses { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
