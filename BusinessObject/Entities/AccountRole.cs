using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class AccountRole
    {
        [Required]
        public int RoleID { get; set; }
        public Role Role { get; set; }

        [Required]
        public Guid AccountID { get; set; }
        public Account Account { get; set; }
    }
}
