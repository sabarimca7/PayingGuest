using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
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
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserDto>> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Check duplicate in business table
                if (await _userRepository.EmailExistsAsync(request.EmailAddress))
                {
                    return ApiResponse<UserDto>
                        .ErrorResponse("Email already exists");
                }

                // 2️⃣ Create Identity user
                var identityUserId = await _identityService.CreateUserAsync(
                    request.EmailAddress,
                    request.Password,
                    request.FirstName,
                    request.LastName,
                    request.UserType);

                //if (string.IsNullOrEmpty(identityUserId))
                //{
                //    return ApiResponse<UserDto>
                //        .ErrorResponse("Identity user creation failed");
                //}

                // 3️⃣ Create Business user
                var user = new User
                {
                    IdentityServerUserId = identityUserId, // 🔥 LINK HERE
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailAddress = request.EmailAddress,
                    PhoneNumber = request.PhoneNumber,
                    UserType = request.UserType,
                    PropertyId = request.PropertyId,
                    IsActive = true,
                    IsOnboarded = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var dto = new UserDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    UserType = user.UserType,
                    PropertyId = user.PropertyId,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    IdentityServerUserId = user.IdentityServerUserId,
                    Roles = new List<string> { user.UserType } ,
                    RoleIds = new List<int>()                                                                   
                };

                return ApiResponse<UserDto>
                    .SuccessResponse(dto, "User created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>
                    .ErrorResponse($"User creation failed: {ex.Message}");
            }
        }
    }
}

