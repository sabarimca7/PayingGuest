using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Exceptions;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RegisterUserCommandHandler> logger,
            IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _identityService = identityService;
        }

        public async Task<ApiResponse<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
              //  await _unitOfWork.BeginTransactionAsync();
                // Check if email already exists
                if (await _unitOfWork.Users.EmailExistsAsync(request.RegisterUserDto.EmailAddress))
                {
                    return ApiResponse<UserDto>.ErrorResponse("Email address already exists");
                }

                // Check if property exists
                var property = await _unitOfWork.Properties.GetByIdAsync(request.RegisterUserDto.PropertyId);
                if (property == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Property not found");
                }
                // Create user in IdentityServer
                var identityServerUserId = await _identityService.CreateUserAsync(
                    request.RegisterUserDto.EmailAddress,
                    request.RegisterUserDto.Password,
                    request.RegisterUserDto.FirstName,
                    request.RegisterUserDto.LastName
                );
                if (identityServerUserId <= 0)
                {
                  //  await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<UserDto>.ErrorResponse("Failed to create user in Identity Server");
                }

                // Map and create user
                var user = _mapper.Map<User>(request.RegisterUserDto);
                user.CreatedDate = DateTime.UtcNow;
                user.IdentityServerUserId = identityServerUserId;
                user.IsActive = true;

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                // Assign roles
                if (request.RegisterUserDto.RoleIds != null && request.RegisterUserDto.RoleIds.Any())
                {
                    foreach (var roleId in request.RegisterUserDto.RoleIds)
                    {
                        var userRole = new UserRole
                        {
                            UserId = user.UserId,
                            RoleId = roleId,
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        };
                       await _unitOfWork.UserRoles.AddAsync(userRole);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                
             

                // Load property for response
                user.Property = property;
                var userDto = _mapper.Map<UserDto>(user);

                // Create audit log
                var auditLog = new AuditLog
                {
                    TableName = "User",
                    RecordId = user.UserId,
                    Action = "INSERT",
                    NewValues = System.Text.Json.JsonSerializer.Serialize(userDto),
                    UserName = "System",
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.AuditLogs.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync();
                // await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<UserDto>.SuccessResponse(userDto, "User registered successfully");
            }
            catch (Exception ex)
            {
               // await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error registering user");
                throw;
            }
        }
    }
}