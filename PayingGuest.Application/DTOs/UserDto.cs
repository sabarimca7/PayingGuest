namespace PayingGuest.Application.DTOs;

public class UserDto
{
    public int? UserId { get; set; }
    public int PropertyId { get; set; }
    public string UserType { get; set; } = "Tenant";
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
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
    public DateTime CreatedDate { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public int? IdentityServerUserId { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<int> RoleIds { get; set; } = new();
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
}
