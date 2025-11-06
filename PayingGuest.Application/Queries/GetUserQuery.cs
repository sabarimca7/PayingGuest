using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;

namespace PayingGuest.Application.Queries
{
    public class GetUserQuery : IRequest<ApiResponse<UserDto>>
    {
        public int UserId { get; set; }
    }
}