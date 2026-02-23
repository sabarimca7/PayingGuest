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
    public class UpdatePropertyCommandHandler
      : IRequestHandler<UpdatePropertyCommand, bool>
    {
        private readonly IPropertyRepository _repository;

        public UpdatePropertyCommandHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var property = new Property
            {
                PropertyId = dto.PropertyId,
                PropertyName = dto.PropertyName,
                PropertyCode = dto.PropertyCode,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                ContactNumber = dto.ContactNumber,
                EmailAddress = dto.EmailAddress,
                TotalFloors = dto.TotalFloors,
                TotalRooms = dto.TotalRooms,
                PropertyType = dto.PropertyType
            };

            await _repository.UpdateAsync(property);
            return true;
        }
    }
}
