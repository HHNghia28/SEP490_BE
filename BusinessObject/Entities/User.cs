using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class User
    {
        [Key]
        public Guid ID { get; set; }

        [StringLength(50)]
        public string Fullname { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(10)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Avatar { get; set; }
    }
}
