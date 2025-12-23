using MediatR;
using PayingGuest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class DeleteUserCommand : IRequest<ApiResponse<bool>>
    {
        public int UserId { get; set; }
    }
}
