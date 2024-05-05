using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class AccountRole
    {
        [Key, Column(Order = 0)]
        public Guid RoleID { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }

        [Key, Column(Order = 1)]
        public string AccountID { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }
    }
}
