using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Slot
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public int SlotNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Detail { get; set; }
    }
}
