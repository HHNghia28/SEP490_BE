using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class StudentClasses
    {
        [Key, Column(Order = 0)]
        public Guid ClassID { get; set; }

        [Key, Column(Order = 1)]
        public string StudentID { get; set; }

        // Navigation properties
        [ForeignKey("ClassID")]
        public virtual Classes Classes { get; set; }

        [ForeignKey("StudentID")]
        public virtual AccountStudent AccountStudent { get; set; }
    }
}
