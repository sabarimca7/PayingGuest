using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string UserType { get; set; }
        public int PropertyId { get; set; }
    }
}
