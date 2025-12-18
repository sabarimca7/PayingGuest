using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IBedRepository
    {
        Task<List<Bed>> GetBedsByPropertyAndRoomAsync(int propertyId, int roomId);
    }
}
  