using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;

namespace PayingGuest.Application.Commands
{
    public class RegisterUserCommand : IRequest<ApiResponse<UserDto>>
    {
        public RegisterUserDto RegisterUserDto { get; set; } = null!;
    }
}