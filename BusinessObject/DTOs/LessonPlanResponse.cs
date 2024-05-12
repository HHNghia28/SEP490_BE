using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class LessonPlanResponse
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public int Slot { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }
    }
}
