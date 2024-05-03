using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class AccountPermission
    {
        [Key, Column(Order = 0)]
        public Guid PermissionID { get; set; }

        [ForeignKey("PermissionID")]
        public Permission Permission { get; set; }

        [Key, Column(Order = 1)]
        public Guid AccountID { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }
    }
}
