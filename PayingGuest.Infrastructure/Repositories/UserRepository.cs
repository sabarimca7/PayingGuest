using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;

namespace PayingGuest.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(PayingGuestDbContext context) : base(context)
        {
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Property)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Property)
                .FirstOrDefaultAsync(u => u.EmailAddress == email);
        }

        public async Task<IEnumerable<User>> GetByPropertyAndTypeAsync(int propertyId, string userType)
        {
            return await _dbSet
                .Include(u => u.Property)
                .Where(u => u.PropertyId == propertyId && u.UserType == userType && u.IsActive)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.EmailAddress == email);
        }
        public async Task<User?> GetByIdentityServerUserIdAsync(int identityServerUserId)
        {
            return await _dbSet
             .Include(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role)
             .FirstOrDefaultAsync(u => u.IdentityServerUserId == identityServerUserId && u.IsActive);
        }

        public async Task<User?> GetUserWithRolesAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
        }
    }
}