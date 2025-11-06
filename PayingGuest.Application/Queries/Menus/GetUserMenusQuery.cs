using MediatR;
using PayingGuest.Application.DTOs.Menus;
using PayingGuest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries.Menus
{
    public class GetUserMenusQuery : IRequest<ApiResponse<List<MenuDto>>>
    {
        public int UserId { get; set; }
    }
}
