using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AttendenceRequest
    {
        [Required(ErrorMessage = "Mã điểm danh là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã điểm danh không được vượt quá 50 ký tự")]
        public string AttendenceID { get; set; }
        public bool Present { get; set; } = true;
        public bool Confirmed { get; set; } = true;
        public string? Note { get; set; } = "";
    }
}
