using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class ActivityLog
    {
        [Key]
        public Guid ID { get; set; }

        [ForeignKey("Account")]
        public Guid AccountID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(250)]
        public string Note { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
    }
}
