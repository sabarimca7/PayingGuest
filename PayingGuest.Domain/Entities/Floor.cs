using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Floor
    {
        public int FloorId { get; set; }          // PK

        public int PropertyId { get; set; }       // FK → Property

        public int FloorNumber { get; set; }

        public string FloorName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        // 🔗 Navigation Properties
        public Property Property { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
