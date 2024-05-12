using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SubjectRequest
    {
        [Required(ErrorMessage = "Tên môn học không được bỏ trống")]
        [MaxLength(50, ErrorMessage = "Tên môn học không được vượt quá 50 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Khối không được bỏ trống")]
        [MaxLength(50, ErrorMessage = "Khối không được vượt quá 50 ký tự")]
        public string Grade { get; set; }

        public List<ComponentScoreRequest> ComponentScores { get; set; }

        public List<LessonPlanRequest> LessonPlans { get; set; }
    }
}
