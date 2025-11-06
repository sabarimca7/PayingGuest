using PayingGuest.Application.DTOs.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetUserMenusAsync(int userId);
        Task<List<MenuDto>> GetMenusByRoleAsync(int roleId);
    }

}
