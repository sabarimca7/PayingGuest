using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class BedDto
    {
        public int BedId { get; set; }
        public string BedNumber { get; set; } = string.Empty;
        //public bool IsAvailable { get; set; }
    }
}
