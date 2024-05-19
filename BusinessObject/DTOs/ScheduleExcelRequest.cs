using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ScheduleExcelRequest
    {
        [Required(ErrorMessage = "File không được bỏ trống")]
        public IFormFile? ScheduleFile { get; set; }
    }
}
