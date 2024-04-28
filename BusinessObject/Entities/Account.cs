using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Account
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public User User { get; set; }
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires {  get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public ICollection<AccountPermission> Permissions { get; set; }
        public ICollection<AccountRole> Roles { get; set; }
    }
}
