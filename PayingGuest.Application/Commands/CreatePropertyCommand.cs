using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public record CreatePropertyCommand(
       string PropertyName,
       string PropertyCode,
       string PropertyType,
       string Address,
       string City,
       string State,
       string Country,
       string PostalCode,
       string ContactNumber,
       string EmailAddress,
       int TotalFloors,
       int TotalRooms,
       bool IsActive,
       string? Status,
       string? Description
   ) : IRequest<int>;
}