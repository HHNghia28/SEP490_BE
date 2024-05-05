﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Guid? UserID { get; set; }

        [Required]
        public Guid RoleID { get; set; }

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

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual Student Student { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}
