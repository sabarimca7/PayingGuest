using MediatR;
using Microsoft.AspNetCore.Http;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetProfileHandler : IRequestHandler<GetProfileQuery, ProfileSummaryDto>
    {
        private readonly IProfileRepository _profileRepository;

        public GetProfileHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<ProfileSummaryDto> Handle(
            GetProfileQuery request,
            CancellationToken cancellationToken)
        {
            return await _profileRepository
                .GetUserProfileSummaryAsync(request.UserId);
        }
    }
}