using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [ForeignKey("Account")]
        public string CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public virtual Account Account { get; set; }
    }
}
