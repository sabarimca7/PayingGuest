using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class MaintenanceRequestDto
    {
        public int MaintenanceRequestId { get; set; }
        public string RequestNumber { get; set; }
        public string Issue { get; set; }
        public string Description { get; set; }
        public DateTime DateRaised { get; set; }
        public string Status { get; set; }
    }
}
