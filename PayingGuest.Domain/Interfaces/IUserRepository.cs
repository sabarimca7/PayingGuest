using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByPropertyAndTypeAsync(int propertyId, string userType);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByIdentityServerUserIdAsync(int identityServerUserId);
        Task<User?> GetUserWithRolesAsync(int userId);
        Task DeleteAsync(User user);
        Task DeleteUserWithBookingsAsync(int userId); // Hard delete with FK cleanup
        Task<User> AddAsync(User user);
    }
}
