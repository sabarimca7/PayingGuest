using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Interfaces;

namespace PayingGuest.Application.Queries
{
    public class GetUsersByTypeQueryHandler : IRequestHandler<GetUsersByTypeQuery, ApiResponse<IEnumerable<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersByTypeQueryHandler> _logger;

        public GetUsersByTypeQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetUsersByTypeQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> Handle(
            GetUsersByTypeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting users for property {PropertyId} with type {UserType}",
                    request.PropertyId, request.UserType);

                // Validate property exists
                var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId);
                if (property == null)
                {
                    return ApiResponse<IEnumerable<UserDto>>.ErrorResponse(
                        $"Property with ID {request.PropertyId} not found");
                }

                // Get users by property and type
                var users = await _unitOfWork.Users.GetByPropertyAndTypeAsync(
                    request.PropertyId,
                    request.UserType);

                var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

                var count = userDtos.Count();
                _logger.LogInformation("Found {Count} users of type {UserType} in property {PropertyId}",
                    count, request.UserType, request.PropertyId);

                return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
                    userDtos,
                    $"Found {count} {request.UserType} user(s)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by type");
                return ApiResponse<IEnumerable<UserDto>>.ErrorResponse(
                    "An error occurred while retrieving users");
            }
        }
    }
}