using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Bed
    {
        public int BedId { get; set; }
        public int RoomId { get; set; }

        public string BedNumber { get; set; }
        public string BedType { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }

        // 🔗 Navigation Property (FK → Room)
        public Room Room { get; set; }
    }
}
