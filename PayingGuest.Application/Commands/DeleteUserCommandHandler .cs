using MediatR;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<bool>> Handle( DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                return ApiResponse<bool>.ErrorResponse("User not found");

            user.IsActive = false; // 👈 SOFT DELETE
            await _userRepository.UpdateAsync(user);

            return ApiResponse<bool>.SuccessResponse(true, "User deactivated successfully");
        }
    }
}
