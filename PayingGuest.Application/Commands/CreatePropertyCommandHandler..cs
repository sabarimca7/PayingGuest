using MediatR;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CreatePropertyCommandHandler
     : IRequestHandler<CreatePropertyCommand, int>
    {
        private readonly IPropertyRepository _repository;

        public CreatePropertyCommandHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = new Property
            {
                PropertyName = request.PropertyName,
                PropertyCode=request.PropertyCode,
                PropertyType=request.PropertyType,
                EmailAddress = request.EmailAddress,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country=request.Country,
                PostalCode = request.PostalCode,
                ContactNumber = request.ContactNumber,
                TotalRooms = request.TotalRooms,
                TotalFloors=request.TotalFloors,
                IsActive=request.IsActive,
                Status = request.Status,
                Description = request.Description,
            };
            await _repository.AddAsync(property);

            return property.PropertyId;
        }
    }
}
