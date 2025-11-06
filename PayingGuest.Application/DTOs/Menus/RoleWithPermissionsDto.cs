using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs.Menus
{
    public class RoleWithPermissionsDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<MenuPermissionDto> MenuPermissions { get; set; } = new();
    }

    public class MenuPermissionDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
    public class AssignMenuToRoleDto
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }

}
