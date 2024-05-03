using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class User
    {
        [Key]
        public Guid ID { get; set; }

        [MaxLength(250)]
        public string Fullname { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [MaxLength(10)]
        public string Nation { get; set; }

        public string Avatar { get; set; }

        public bool? IsBachelor { get; set; }

        public bool? IsMaster { get; set; }

        public bool? IsDoctor { get; set; }

        public bool? IsProfessor { get; set; }

        public Account Account { get; set; }
    }
}
