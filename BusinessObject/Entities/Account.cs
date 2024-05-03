using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Account
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Guid UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [MaxLength(100)]
        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        [Required]
        [MaxLength(50)]
        public string CreateBy { get; set; }

        [Required]
        [MaxLength(50)]
        public string UpdateBy { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public DateTime UpdateAt { get; set; }

        public ICollection<AccountRole> AccountRoles { get; set; }
        public ICollection<AccountPermission> AccountPermissions { get; set; }
    }
}
