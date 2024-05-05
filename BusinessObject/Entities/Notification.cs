using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Notification
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Thumbnail { get; set; }

        [Required]
        [MaxLength(50)]
        public string CreateBy { get; set; }

        [Required]
        [MaxLength(50)]
        public string UpdateBy { get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
