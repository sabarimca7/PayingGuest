using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetOnboardedUsersQueryHandler
    : IRequestHandler<GetOnboardedUsersQuery, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetOnboardedUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> Handle(
            GetOnboardedUsersQuery request,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository
                .GetOnboardedUsersByPropertyAsync(request.PropertyId);

            return users.Select(u => new UserDto
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailAddress = u.EmailAddress,
                PhoneNumber = u.PhoneNumber
            }).ToList();
        }
    }
}
