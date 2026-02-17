using MediatR;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class UpdateProfileCommandHandler: IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IProfileRepository _profileRepository;

        public UpdateProfileCommandHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<bool> Handle(UpdateProfileCommand request,CancellationToken cancellationToken)
        {
            var dto = request.Profile;

            var user = await _profileRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception("User not found");

            // ✅ Split full name safely
            var nameParts = dto.Name?.Trim().Split(' ', 2);

            user.FirstName = nameParts?[0] ?? user.FirstName;
            user.LastName = nameParts?.Length > 1 ? nameParts[1] : user.LastName;

            user.PhoneNumber = dto.Phone;
            user.EmailAddress = dto.Email;

            await _profileRepository.UpdateProfileAsync(user);

            return true;
        }
    }
}