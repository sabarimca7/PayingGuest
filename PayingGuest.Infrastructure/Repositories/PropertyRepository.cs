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
    public class PropertyRepository : IPropertyRepository
    {
        private readonly PayingGuestDbContext _context;

        public PropertyRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Property property)
        {
            await _context.Property.AddAsync(property);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Property>> GetAllAsync()
        {
            return await _context.Property
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.Property
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PropertyId == id);
        }
        public async Task DeleteAsync(Property property)
        {
            _context.Property.Remove(property);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Property property)
        {
            _context.Property.Remove(property);
            await _context.SaveChangesAsync();
        }
    }
}
