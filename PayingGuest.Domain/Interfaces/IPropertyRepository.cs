using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{

    public interface IPropertyRepository
    {
        Task AddAsync(Property property);
        Task<List<Property>> GetAllAsync();
        Task<Property?> GetByIdAsync(int id);
        Task DeleteAsync(Property property);
        Task UpdateAsync(Property property);
    }
}
