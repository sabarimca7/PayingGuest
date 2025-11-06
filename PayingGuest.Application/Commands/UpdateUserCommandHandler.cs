using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Exceptions;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;

namespace PayingGuest.Application.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateUserCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating user with ID: {UserId}", request.UserId);

                // Retrieve existing user
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
                    return ApiResponse<UserDto>.ErrorResponse($"User with ID {request.UserId} not found");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Attempt to update inactive user with ID: {UserId}", request.UserId);
                    return ApiResponse<UserDto>.ErrorResponse("Cannot update inactive user");
                }

                // Store old values for audit
                var oldValues = JsonSerializer.Serialize(new
                {
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.AlternatePhoneNumber,
                    user.DateOfBirth,
                    user.Gender,
                    user.BloodGroup,
                    user.EmergencyContactName,
                    user.EmergencyContactNumber,
                    user.PermanentAddress,
                    user.CurrentAddress,
                    user.Occupation,
                    user.Company,
                    user.AadharNumber,
                    user.PanNumber
                });

                // Update user properties
                user.FirstName = request.UpdateUserDto.FirstName;
                user.LastName = request.UpdateUserDto.LastName;
                user.PhoneNumber = request.UpdateUserDto.PhoneNumber;
                user.AlternatePhoneNumber = request.UpdateUserDto.AlternatePhoneNumber;
                user.DateOfBirth = request.UpdateUserDto.DateOfBirth;
                user.Gender = request.UpdateUserDto.Gender;
                user.BloodGroup = request.UpdateUserDto.BloodGroup;
                user.EmergencyContactName = request.UpdateUserDto.EmergencyContactName;
                user.EmergencyContactNumber = request.UpdateUserDto.EmergencyContactNumber;
                user.PermanentAddress = request.UpdateUserDto.PermanentAddress;
                user.CurrentAddress = request.UpdateUserDto.CurrentAddress;
                user.Occupation = request.UpdateUserDto.Occupation;
                user.Company = request.UpdateUserDto.Company;
                user.AadharNumber = request.UpdateUserDto.AadharNumber;
                user.PanNumber = request.UpdateUserDto.PanNumber;
                user.LastModifiedDate = DateTime.UtcNow;
                user.LastModifiedBy = _currentUserService.UserName ?? "System";

                // Store new values for audit
                var newValues = JsonSerializer.Serialize(new
                {
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.AlternatePhoneNumber,
                    user.DateOfBirth,
                    user.Gender,
                    user.BloodGroup,
                    user.EmergencyContactName,
                    user.EmergencyContactNumber,
                    user.PermanentAddress,
                    user.CurrentAddress,
                    user.Occupation,
                    user.Company,
                    user.AadharNumber,
                    user.PanNumber
                });

                // Update in database
                await _unitOfWork.Users.UpdateAsync(user);

                // Create audit log
                var auditLog = new AuditLog
                {
                    TableName = "User",
                    RecordId = user.UserId,
                    Action = "UPDATE",
                    OldValues = oldValues,
                    NewValues = newValues,
                    UserId = _currentUserService.UserId != null ? int.Parse(_currentUserService.UserId) : null,
                    UserName = _currentUserService.UserName ?? "System",
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.AuditLogs.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO
                var userDto = _mapper.Map<UserDto>(user);

                _logger.LogInformation("User updated successfully with ID: {UserId}", user.UserId);
                return ApiResponse<UserDto>.SuccessResponse(userDto, "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", request.UserId);
                return ApiResponse<UserDto>.ErrorResponse($"An error occurred while updating the user: {ex.Message}");
            }
        }
    }
}
