using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Student
    {
        [Key]
        [MaxLength(50)]
        public Guid ID { get; set; }

        [MaxLength(250)]
        public string Fullname { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Birthday { get; set; }

        [MaxLength(250)]
        public string Birthplace { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        [MaxLength(10)]
        public string Nation { get; set; }

        public bool? IsMartyrs { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(250)]
        public string HomeTown { get; set; }

        [MaxLength(250)]
        public string Avatar { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

        [MaxLength(50)]
        public string FatherFullName { get; set; }

        [MaxLength(50)]
        public string FatherProfession { get; set; }

        [MaxLength(10)]
        public string FatherPhone { get; set; }

        [MaxLength(50)]
        public string MotherFullName { get; set; }

        [MaxLength(50)]
        public string MotherProfession { get; set; }

        [MaxLength(10)]
        public string MotherPhone { get; set; }

        // Navigation property
        public virtual ICollection<AccountStudent> AccountStudents { get; set; }
    }
}
