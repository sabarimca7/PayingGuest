using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IProfileRepository
    {
      
        Task<ProfileSummaryDto> GetUserProfileSummaryAsync(int userId);

        Task UpdateProfileAsync(User user);
        Task<User?> GetByIdAsync(int userId);
    }
}
