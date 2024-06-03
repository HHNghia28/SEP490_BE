using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class NotificationRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public IFormFile? Thumbnail { get; set; }
    }
}
