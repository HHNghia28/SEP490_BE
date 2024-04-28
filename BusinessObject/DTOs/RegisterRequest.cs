using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mật khẩu không được vượt quá 50 ký tự")]
        public string Password { get; set; }

        [StringLength(50, ErrorMessage = "Họ tên không được vượt quá 50 ký tự")]
        public string Fullname { get; set; }

        [StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        [RegularExpression("^0[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
