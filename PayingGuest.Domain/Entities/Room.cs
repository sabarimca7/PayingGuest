using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Room
    {
        public int RoomId { get; set; }
        public int FloorId { get; set; }

        // 🔗 Foreign Key → Property
        public int PropertyId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; }
        public int TotalBeds { get; set; }
        public decimal RentPerBed { get; set; }
        public decimal SecurityDeposit { get; set; }
        public string? Amenities { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }

        // Navigation Properties
       // public Floor Floor { get; set; }
        public Property Property { get; set; }
        public ICollection<Bed> Beds { get; set; }

        
    }
}

