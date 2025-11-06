using PayingGuest.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuTitle { get; set; } = string.Empty;
        public string? MenuUrl { get; set; }
        public string? MenuIcon { get; set; }
        public int? ParentMenuId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public virtual Menu? ParentMenu { get; set; }
        public virtual ICollection<Menu> SubMenus { get; set; } = new List<Menu>();
        public virtual ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = new List<RoleMenuPermission>();
    }
}
