using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId);
        Task<IEnumerable<Menu>> GetMenusByRoleIdsAsync(List<int> roleIds);
    }
}
