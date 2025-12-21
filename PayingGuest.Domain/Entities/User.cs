using MediatR;
using PayingGuest.Domain.Common;
using System.Reflection.Metadata;

namespace PayingGuest.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternatePhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public string? PermanentAddress { get; set; }
    public string? CurrentAddress { get; set; }
    public string? Occupation { get; set; }
    public string? Company { get; set; }
    public string? AadharNumber { get; set; }
    public string? PanNumber { get; set; }
    public bool IsActive { get; set; }
    public int? IdentityServerUserId { get; set; }
    public Property? Property { get; set; } = null!;
  
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Booking> Booking { get; set; } = new List<Booking>();

}
