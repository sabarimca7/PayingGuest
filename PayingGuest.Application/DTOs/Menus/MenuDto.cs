using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs.Menus
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string MenuUrl { get; set; } = string.Empty;
        public string? RouteUrl { get; set; }
        public int? ParentMenuId { get; set; }
        public int SortOrder { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public List<MenuDto> SubMenus { get; set; } = new();
        public MenuPermissionDto Permissions { get; set; } = new ();
        public int DisplayOrder { get; set; }
    }
}
