using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Schedule
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ClassID { get; set; }
        public Guid SubjectID { get; set; }
        public string TeacherID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int SlotByDate { get; set; }
        public int SlotByLessonPlans { get; set; }

        [Required]
        [MaxLength(10)]
        public string Rank { get; set; }

        [Required]
        [MaxLength(250)]
        public string Note { get; set; }

        [ForeignKey("ClassID")]
        public virtual Classes Classes { get; set; }

        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; }

        [ForeignKey("TeacherID")]
        public virtual Account Teacher { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }
}
