using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ImageRequest
    {
        [Required(ErrorMessage = "File không được bỏ trống")]
        public IFormFile? File { get; set; }
        public string? Name { get; set; }
        public string? Folder {  get; set; }
    }
}
