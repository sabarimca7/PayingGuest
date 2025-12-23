using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetProfileQuery : IRequest<ProfileSummaryDto>
    {
        public int UserId { get; set; }
    }
}
