using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? ClientId { get; }
        bool IsAuthenticated { get; }
        string? UserType { get; }
        string? IpAddress { get; }
    }

}
