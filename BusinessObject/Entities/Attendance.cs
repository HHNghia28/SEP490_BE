using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Attendance
    {
        [Key]
        public Guid ID { get; set; }

        public Guid ScheduleID { get; set; }

        [MaxLength(50)]
        public string StudentID { get; set; }

        public bool Present { get; set; } = false;
        public bool Confirmed { get; set; } = true;

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ScheduleID")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("StudentID")]
        public virtual AccountStudent AccountStudent { get; set; }
    }
}
