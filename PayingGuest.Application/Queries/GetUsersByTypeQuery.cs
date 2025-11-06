using System.Collections.Generic;
using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;

namespace PayingGuest.Application.Queries
{
    public class GetUsersByTypeQuery : IRequest<ApiResponse<IEnumerable<UserDto>>>
    {
        public int PropertyId { get; set; }
        public string UserType { get; set; } = string.Empty;
    }
}