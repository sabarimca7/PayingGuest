using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetPropertyByIdQueryHandler
       : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
    {
        private readonly IPropertyRepository _repository;

        public GetPropertyByIdQueryHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PropertyDto?> Handle(
            GetPropertyByIdQuery request,
            CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.Id);

            if (property == null)
                return null;

            return new PropertyDto
            {
                PropertyId = property.PropertyId,
                PropertyCode=property.PropertyCode,
                PropertyName = property.PropertyName,
                EmailAddress = property.EmailAddress,
                Address = property.Address,
                City = property.City,
                State = property.State,
                Country=property.Country,
                PostalCode = property.PostalCode,
                ContactNumber = property.ContactNumber,
                TotalRooms = property.TotalRooms,
                TotalFloors=property.TotalFloors,
                Status = property.Status,
                PropertyType=property.PropertyType,
                Description = property.Description
            };
        }
    }
}
