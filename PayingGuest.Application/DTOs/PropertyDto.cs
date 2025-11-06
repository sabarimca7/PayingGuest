using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class PropertyDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int TotalFloors { get; set; }
        public int TotalRooms { get; set; }
        public string PropertyType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    public class CreatePropertyDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int TotalFloors { get; set; }
        public int TotalRooms { get; set; }
        public string PropertyType { get; set; } = string.Empty;
    }

    public class UpdatePropertyDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int TotalFloors { get; set; }
        public int TotalRooms { get; set; }
        public string PropertyType { get; set; } = string.Empty;

    }
}
