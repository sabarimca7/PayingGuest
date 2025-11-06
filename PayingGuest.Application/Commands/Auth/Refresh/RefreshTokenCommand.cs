using MediatR;
using PayingGuest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands.Auth.Refresh
{
    public class RefreshTokenCommand : IRequest<ApiResponse<TokenResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
