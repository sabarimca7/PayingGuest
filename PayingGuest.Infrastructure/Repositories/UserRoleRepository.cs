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
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(PayingGuestDbContext context) : base(context)
        {
        }

        public async Task<List<UserRole>> GetUserRolesByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UserHasRoleAsync(int userId, int roleId)
        {
            return await _dbSet
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);
        }

        public async Task RemoveUserRolesAsync(int userId)
        {
            var userRoles = await _dbSet
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            foreach (var userRole in userRoles)
            {
                userRole.IsActive = false;
            }
        }
    }
}
