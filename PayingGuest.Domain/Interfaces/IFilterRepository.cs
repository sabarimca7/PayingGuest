using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IFilterRepository
    {
        Task<List<Room>> FilterRoomsAsync(
            int propertyId,
            decimal? minPrice,
            decimal? maxPrice,
            int? Capacity
        );
    }
}