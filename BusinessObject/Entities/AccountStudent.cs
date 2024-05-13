using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessObject.Entities
{
    public class AccountStudent
    {
        [Key]
        [MaxLength(50)]
        public string ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }
        public Guid? UserID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [MaxLength(100)]
        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Student Student { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}
