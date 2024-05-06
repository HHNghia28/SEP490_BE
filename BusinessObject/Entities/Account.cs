using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Account
    {
        [Key]
        [MaxLength(50)]
        public string ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(250)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Guid UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [MaxLength(100)]
        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        public ICollection<AccountRole> AccountRoles { get; set; }
        public ICollection<AccountPermission> AccountPermissions { get; set; }
    }
}
