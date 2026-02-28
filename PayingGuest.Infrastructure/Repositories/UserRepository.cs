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
        // 🔹 Get user by ID
        //public async Task<User?> GetByIdAsync(int id)
        //{
        //    return await _context.User
        //        .FirstOrDefaultAsync(u => u.UserId == id);
        //}

        //// 🔹 Get only active users (Soft delete filter)
        //public async Task<List<User>> GetAllActiveAsync()
        //{
        //    return await _context.User
        //        .Where(u => u.IsActive)
        //        .OrderByDescending(u => u.UserId)
        //        .ToListAsync();
        //}

        // 🔹 Update user / Soft delete
        public async Task UpdateAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }

        // 🔹 Hard delete user ONLY (no bookings)
        public async Task DeleteAsync(User user)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }

        // 🔹 Hard delete user + bookings (OPTION 4)
        public async Task DeleteUserWithBookingsAsync(int userId)
        {
            var bookings = await _context.Booking
                .Where(b => b.UserId == userId)
                .ToListAsync();

            if (bookings.Any())
                _context.Booking.RemoveRange(bookings);

            var user = await _context.User.FindAsync(userId);
            if (user != null)
                _context.User.Remove(user);

            await _context.SaveChangesAsync();
        }
        public async Task<User> AddAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<List<User>> GetOnboardedUsersByPropertyAsync(int propertyId)
        {
            return await _context.User
                .Where(u => u.PropertyId == propertyId && u.IsOnboarded)
                .ToListAsync();
        }
    }

}
