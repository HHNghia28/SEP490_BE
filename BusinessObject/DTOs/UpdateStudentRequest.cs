using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UpdateStudentRequest
    {

        public string Password { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ tên không được vượt quá 50 ký tự")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự")]
        public string Email { get; set; } = "";

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

        [Required(ErrorMessage = "Nơi sinh là bắt buộc")]
        [MaxLength(250, ErrorMessage = "Nơi sinh không được vượt quá 250 ký tự")]
        public string Birthplace { get; set; }

        [MaxLength(250, ErrorMessage = "Quê quán không được vượt quá 250 ký tự")]
        public string HomeTown { get; set; } = "";

        [MaxLength(50, ErrorMessage = "Họ tên cha không được vượt quá 50 ký tự")]
        public string? FatherFullName { get; set; } = "";

        [MaxLength(50, ErrorMessage = "Nghề nghiệp cha không được vượt quá 50 ký tự")]
        public string? FatherProfession { get; set; } = "";

        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        [RegularExpression("^(0[0-9]{9}|)$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? FatherPhone { get; set; } = "";

        [MaxLength(50, ErrorMessage = "Họ tên mẹ không được vượt quá 50 ký tự")]
        public string? MotherFullName { get; set; } = "";

        [MaxLength(50, ErrorMessage = "Nghề nghiệp mẹ không được vượt quá 50 ký tự")]
        public string? MotherProfession { get; set; } = "";

        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        [RegularExpression("^(0[0-9]{9}|)$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? MotherPhone { get; set; } = "";
        public IFormFile? Avatar { get; set; }
        public bool? IsMartyrs { get; set; }
    }
}
