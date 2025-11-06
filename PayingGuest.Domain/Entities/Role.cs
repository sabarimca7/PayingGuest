using PayingGuest.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = new List<RoleMenuPermission>();
    }
}
