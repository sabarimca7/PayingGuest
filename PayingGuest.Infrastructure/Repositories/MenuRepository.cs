using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(PayingGuestDbContext context) : base(context)
        {
        }

       public async Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId)
    {
        var menus = await (from menu in _context.Menus
                          join rmp in _context.RoleMenuPermissions on menu.MenuId equals rmp.MenuId
                          join role in _context.Roles on rmp.RoleId equals role.RoleId
                          join ur in _context.UserRoles on role.RoleId equals ur.RoleId
                          where ur.UserId == userId && menu.IsActive && rmp.IsActive && rmp.CanView
                          select menu)
                          .Distinct()
                          .OrderBy(m => m.DisplayOrder)
                          .ToListAsync();

        return menus;
    }

    public async Task<IEnumerable<Menu>> GetMenusByRoleIdsAsync(List<int> roleIds)
    {
        var menus = await (from menu in _context.Menus
                          join rmp in _context.RoleMenuPermissions on menu.MenuId equals rmp.MenuId
                          where roleIds.Contains(rmp.RoleId) && menu.IsActive && rmp.IsActive && rmp.CanView
                          select menu)
                          .Distinct()
                          .OrderBy(m => m.DisplayOrder)
                          .ToListAsync();

        return menus;
    }
    }
}
