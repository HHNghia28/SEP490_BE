using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class RolePermission
    {
        [Key, Column(Order = 0)]
        public int PermissionID { get; set; }

        [ForeignKey("PermissionID")]
        public Permission Permission { get; set; }

        [Key, Column(Order = 1)]
        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }
    }
}
