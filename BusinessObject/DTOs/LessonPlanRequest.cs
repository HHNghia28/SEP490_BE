using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class LessonPlanRequest
    {
        [Required(ErrorMessage = "Tiết học không được bỏ trống")]
        public int Slot { get; set; }

        [Required(ErrorMessage = "Nội dung tiết học không được bỏ trống")]
        [MaxLength(250, ErrorMessage = "Nội dung tiết học không được vượt quá 250 ký tự")]
        public string Title { get; set; }
    }
}
