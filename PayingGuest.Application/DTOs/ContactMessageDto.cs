using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class ContactMessageDto
    {
        public required string YourName { get; set; }
        public required string EmailAddress { get; set; }
        public string? Subject { get; set; }
        public required string Message { get; set; }
    }
}
