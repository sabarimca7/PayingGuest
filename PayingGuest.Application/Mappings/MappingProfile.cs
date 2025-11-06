using AutoMapper;
using PayingGuest.Application.Commands;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.DTOs.Menus;
using PayingGuest.Domain.Entities;

namespace PayingGuest.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.PropertyName,
                    opt => opt.MapFrom(src => src.Property != null ? src.Property.PropertyName : string.Empty));

            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Property, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
                .ForMember(dest => dest.UserType, opt => opt.Ignore())
                .ForMember(dest => dest.EmailAddress, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Property, opt => opt.Ignore());

            // Property mappings
            CreateMap<Property, PropertyDto>();
            CreateMap<CreatePropertyDto, Property>();
            CreateMap<UpdatePropertyDto, Property>();


            CreateMap<Role, RoleDto>()
             //.ForMember(dest => dest.UserCount, opt => opt.MapFrom(src =>
             //    src.UserRoles.Count(ur => ur.IsActive)))
             .ForMember(dest => dest.MenuCount, opt => opt.MapFrom(src =>
                 src.RoleMenuPermissions.Count(rmp => rmp.IsActive)));

            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
               // .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RoleMenuPermissions, opt => opt.Ignore());

            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
              //  .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RoleMenuPermissions, opt => opt.Ignore());

            CreateMap<Role, RoleWithPermissionsDto>()
                .ForMember(dest => dest.MenuPermissions, opt => opt.MapFrom(src =>
                    src.RoleMenuPermissions.Where(rmp => rmp.IsActive)
                        .Select(rmp => new MenuPermissionDto
                        {
                            MenuId = rmp.MenuId,
                            MenuName = rmp.Menu.MenuName,
                            CanView = rmp.CanView,
                            CanCreate = rmp.CanCreate,
                            CanEdit = rmp.CanEdit,
                            CanDelete = rmp.CanDelete
                        }).ToList()));

            // Menu Mappings
            CreateMap<Menu, MenuDto>()
                .ForMember(dest => dest.SubMenus, opt => opt.MapFrom(src => src.SubMenus));
               // .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            // RoleMenuPermission Mappings
            CreateMap<AssignMenuToRoleDto, RoleMenuPermission>()
                .ForMember(dest => dest.RoleMenuPermissionId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Menu, opt => opt.Ignore());

        }
    }
}