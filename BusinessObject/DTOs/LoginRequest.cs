using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mật khẩu không được vượt quá 50 ký tự")]
        public string Password { get; set; }
    }
}
