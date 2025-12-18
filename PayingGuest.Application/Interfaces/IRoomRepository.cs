using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetRoomsByPropertyIdAsync(int propertyId);
    }
}