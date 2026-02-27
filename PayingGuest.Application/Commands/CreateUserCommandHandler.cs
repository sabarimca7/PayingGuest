using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{

    public class CreateUserCommandHandler
        : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            // 🔍 Check duplicate email
            if (await _userRepository.EmailExistsAsync(request.EmailAddress))
            {
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.EmailAddress,
                PhoneNumber = request.PhoneNumber,
                UserType = request.UserType,
                PropertyId = request.PropertyId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);

            var dto = new UserDto
            {
                UserId = createdUser.UserId,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                EmailAddress = createdUser.EmailAddress,
                PhoneNumber = createdUser.PhoneNumber,
                UserType = createdUser.UserType,
                PropertyId = createdUser.PropertyId,
                IsActive = createdUser.IsActive,
                CreatedDate = createdUser.CreatedDate
            };

            return new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User created successfully",
                Data = dto
            };
        }
    }
}
