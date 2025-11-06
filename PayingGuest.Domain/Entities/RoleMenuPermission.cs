using PayingGuest.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class RoleMenuPermission : BaseEntity
    {
        public int RoleMenuPermissionId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool IsActive { get; set; }
        public virtual Role Role { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
    }
}
