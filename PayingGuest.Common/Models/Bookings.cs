using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Common.Models
{
    internal class Bookings
    {
        public class Booking
        {
            public int BookingId { get; set; }
            public string CustomerName { get; set; }
            public int RoomNo { get; set; }
            public DateTime CheckIn { get; set; }
            public DateTime CheckOut { get; set; }
            public string BookingStatus { get; set; }
        }

    }
}
