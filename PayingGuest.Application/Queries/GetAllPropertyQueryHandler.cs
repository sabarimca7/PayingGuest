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
    public class GetAllPropertyQueryHandler
    : IRequestHandler<GetAllPropertyQuery, List<PropertyDto>>
    {
        private readonly IPropertyRepository _repository;

        public GetAllPropertyQueryHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PropertyDto>> Handle(
            GetAllPropertyQuery request,
            CancellationToken cancellationToken)
        {
            var properties = await _repository.GetAllAsync();

            return properties.Select(p => new PropertyDto
            {
                PropertyId = p.PropertyId,
                PropertyName = p.PropertyName,
                PropertyCode = p.PropertyCode,
                Address = p.Address,
                City = p.City,
                State = p.State,
                Country = p.Country,
                PostalCode = p.PostalCode,
                ContactNumber = p.ContactNumber,
                EmailAddress = p.EmailAddress,
                TotalFloors = p.TotalFloors,
                TotalRooms = p.TotalRooms,
                PropertyType = p.PropertyType,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate,
                Status = p.Status,
                Description = p.Description
            }).ToList(); 
        }
    }
}
