using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IRoomAmenitiesRepository
    {
        Task<string?> GetAmenitiesByRoomIdAsync(int roomId);
    }
}
