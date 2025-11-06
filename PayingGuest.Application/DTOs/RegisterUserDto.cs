namespace PayingGuest.Application.DTOs
{
    public class RegisterUserDto
    {
        public int PropertyId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        //  public string? AlternatePhoneNumber { get; set; }
        // public DateTime? DateOfBirth { get; set; }
        //public string? Gender { get; set; }
        //public string? BloodGroup { get; set; }
        //public string? EmergencyContactName { get; set; }
        //public string? EmergencyContactNumber { get; set; }
        //public string? PermanentAddress { get; set; }
        //public string? CurrentAddress { get; set; }
        //public string? Occupation { get; set; }
        //public string? Company { get; set; }
        //public string? AadharNumber { get; set; }
        //public string? PanNumber { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}