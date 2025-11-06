using PayingGuest.Domain.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class RoleMenu : BaseEntity
    {
        public int RoleMenuId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public Role Role { get; set; } = null!;
        public Menu Menu { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
