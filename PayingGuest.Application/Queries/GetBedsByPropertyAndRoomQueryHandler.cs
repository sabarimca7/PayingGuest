using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetBedsByPropertyAndRoomQueryHandler: IRequestHandler<GetBedsByPropertyAndRoomQuery, ApiResponse<List<BedDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBedsByPropertyAndRoomQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<BedDto>>> Handle(GetBedsByPropertyAndRoomQuery request, CancellationToken cancellationToken)
        {
            var beds = await _unitOfWork.Beds
                .GetBedsByPropertyAndRoomAsync(request.PropertyId, request.RoomId);

            if (beds == null || beds.Count == 0)
            {
                return ApiResponse<List<BedDto>>.ErrorResponse(
                    $"No beds found for property {request.PropertyId} and room {request.RoomId}");
            }

            // ⭐ Manual mapping (no AutoMapper)
            var bedDtos = beds.Select(b => new BedDto
            {
                BedId = b.BedId,
                BedNumber = b.BedNumber,
                //IsAvailable = b.Status == "Available" // Adjust according to your DB
            }).ToList();

            return ApiResponse<List<BedDto>>.SuccessResponse(
                bedDtos,
                "Beds fetched successfully."
            );
        }
    }
}