using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public UserInfoDto User { get; set; } = null!;
        public List<MenuItemDto> Menus { get; set; } = new();
    }
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class MenuItemDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? MenuUrl { get; set; }
        public string? Icon { get; set; }
        public int? ParentMenuId { get; set; }
        public int DisplayOrder { get; set; }
    }

}
