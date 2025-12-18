using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class RoomDto
    {
        public int PropertyId { get; set; }
        public int RoomId { get; set; }
        public string Image { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int TotalBeds { get; set; }
        public decimal RentPerBed { get; set; }

        // ⭐ Combined hard-coded feature list
        public List<string> Features { get; set; } = new();
 


    }
}
