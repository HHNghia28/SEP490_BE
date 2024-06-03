using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class NotificationResponse
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Thumbnail { get; set; }

        [ForeignKey("Account")]
        public string CreateBy { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }
    }
}
