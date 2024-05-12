using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClassesRequest
    {
        [Required(ErrorMessage = "Mã giáo viên không được để trống")]
        [MaxLength(50, ErrorMessage = "Mã giáo viên không được vượt quá 50 ký tự")]
        public string TeacherID { get; set; }

        [Required(ErrorMessage = "Năm học không được để trống")]
        [MaxLength(50, ErrorMessage = "Năm học không được vượt quá 50 ký tự")]
        public string SchoolYear { get; set; }

        [Required(ErrorMessage = "Tên lớp không được để trống")]
        [MaxLength(50, ErrorMessage = "Tên lớp không được vượt quá 50 ký tự")]
        public string Classroom { get; set; }
        public List<string> Students { get; set; }
    }
}
