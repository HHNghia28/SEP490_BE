using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ComponentScoreRequest
    {
        [Required(ErrorMessage = "Tên điểm thành phần không được bỏ trống")]
        [MaxLength(250, ErrorMessage = "Tên điểm thành phần không được vượt quá 250 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Hệ số không được bỏ trống")]
        public decimal ScoreFactor { get; set; }

        [Required(ErrorMessage = "Số cột không được bỏ trống")]
        public int Count { get; set; }

        [Required(ErrorMessage = "Học kỳ không được bỏ trống")]
        [MaxLength(250, ErrorMessage = "Học kỳ không được vượt quá 250 ký tự")]
        public string Semester { get; set; }
    }
}
