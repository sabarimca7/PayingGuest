using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<List<UserRole>> GetUserRolesByUserIdAsync(int userId);
        Task<bool> UserHasRoleAsync(int userId, int roleId);
        Task RemoveUserRolesAsync(int userId);
    }
}
