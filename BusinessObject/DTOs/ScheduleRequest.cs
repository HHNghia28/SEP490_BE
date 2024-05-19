using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScheduleRequest
    {
        [Required(ErrorMessage = "Lớp học không được bỏ trống")]
        public Guid ClassID { get; set; }
        [Required(ErrorMessage = "Môn học không được bỏ trống")]
        public Guid SubjectID { get; set; }
        [Required(ErrorMessage = "Giáo viên bộ môn không được bỏ trống")]
        public string TeacherID { get; set; }
        [Required(ErrorMessage = "Ngày học không được bỏ trống")]

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Tiết học không được bỏ trống")]

        public int SlotByDate { get; set; }
        [Required(ErrorMessage = "Tiết theo chương trình học không được bỏ trống")]
        public int SlotByLessonPlans { get; set; }
    }
}
