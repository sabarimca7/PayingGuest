//Menu Service

using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs.Menus;
using PayingGuest.Application.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly PayingGuestDbContext _context;

        public MenuService(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuDto>> GetUserMenusAsync(int userId)
        {
            var menusWithPermissions = await (from menu in _context.Menus
                                              join rmp in _context.RoleMenuPermissions on menu.MenuId equals rmp.MenuId
                                              join role in _context.Roles on rmp.RoleId equals role.RoleId
                                              join ur in _context.UserRoles on role.RoleId equals ur.RoleId
                                              where ur.UserId == userId && menu.IsActive && rmp.IsActive && rmp.CanView
                                              select new
                                              {
                                                  menu.MenuId,
                                                  menu.MenuName,
                                                  menu.MenuUrl,
                                                  menu.MenuIcon,
                                                  menu.ParentMenuId,
                                                  menu.DisplayOrder,
                                                  rmp.CanView,
                                                  rmp.CanCreate,
                                                  rmp.CanEdit,
                                                  rmp.CanDelete
                                              })
                                             .Distinct()
                                             .ToListAsync();

            var menuDtos = menusWithPermissions
                .GroupBy(m => m.MenuId)
                .Select(g => new MenuDto
                {
                    MenuId = g.Key,
                    MenuName = g.First().MenuName,
                    MenuUrl = g.First().MenuUrl,
                    Icon = g.First().MenuIcon,
                    ParentMenuId = g.First().ParentMenuId,
                    DisplayOrder = g.First().DisplayOrder,
                    Permissions = new MenuPermissionDto
                    {
                        CanView = g.Any(x => x.CanView),
                        CanCreate = g.Any(x => x.CanCreate),
                        CanEdit = g.Any(x => x.CanEdit),
                        CanDelete = g.Any(x => x.CanDelete)
                    }
                })
                .OrderBy(m => m.DisplayOrder)
                .ToList();

            return BuildMenuHierarchy(menuDtos);
        }

        public async Task<List<MenuDto>> GetMenusByRoleAsync(int roleId)
        {
            var menusWithPermissions = await (from menu in _context.Menus
                                              join rmp in _context.RoleMenuPermissions on menu.MenuId equals rmp.MenuId
                                              where rmp.RoleId == roleId && menu.IsActive && rmp.IsActive && rmp.CanView
                                              select new MenuDto
                                              {
                                                  MenuId = menu.MenuId,
                                                  MenuName = menu.MenuName,
                                                  MenuUrl = menu.MenuUrl,
                                                  Icon = menu.MenuIcon,
                                                  ParentMenuId = menu.ParentMenuId,
                                                  DisplayOrder = menu.DisplayOrder,
                                                  Permissions = new MenuPermissionDto
                                                  {
                                                      CanView = rmp.CanView,
                                                      CanCreate = rmp.CanCreate,
                                                      CanEdit = rmp.CanEdit,
                                                      CanDelete = rmp.CanDelete
                                                  }
                                              })
                                             .OrderBy(m => m.DisplayOrder)
                                             .ToListAsync();

            return BuildMenuHierarchy(menusWithPermissions);
        }

        private List<MenuDto> BuildMenuHierarchy(List<MenuDto> allMenus)
        {
            var parentMenus = allMenus.Where(m => m.ParentMenuId == null).ToList();

            foreach (var parent in parentMenus)
            {
                parent.SubMenus = allMenus.Where(m => m.ParentMenuId == parent.MenuId).ToList();
            }

            return parentMenus;
        }
    }

}