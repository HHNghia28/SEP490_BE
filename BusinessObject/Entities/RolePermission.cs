using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class RolePermission
    {
        [Required]
        public int PermissionID { get; set; }
        public Permission Permission { get; set; }

        [Required]
        public int RoleID { get; set; }
        public Role Role { get; set; }
    }
}
