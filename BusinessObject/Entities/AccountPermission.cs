using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class AccountPermission
    {
        [Required]
        public int PermissionID { get; set; }
        public Permission Permission { get; set; }

        [Required]
        public Guid AccountID { get; set; }
        public Account Account { get; set; }
    }
}
