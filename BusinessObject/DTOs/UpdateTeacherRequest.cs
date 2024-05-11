using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UpdateTeacherRequest
    {
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mật khẩu không được vượt quá 50 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ tên không được vượt quá 50 ký tự")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        [RegularExpression("^0[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        [MaxLength(10)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Dân tộc là bắt buộc")]
        [MaxLength(10)]
        public string Nation { get; set; }

        public bool? IsBachelor { get; set; }

        public bool? IsMaster { get; set; }

        public bool? IsDoctor { get; set; }

        public bool? IsProfessor { get; set; }

        public IFormFile? Avatar { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
