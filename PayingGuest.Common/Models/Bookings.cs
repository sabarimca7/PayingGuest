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
            public int BookingNo { get; set; }
            //public string BookingType { get; set; }

            //public int RoomNo { get; set; }
            public DateOnly CheckIn { get; set; }
            public DateOnly? CheckOut { get; set; }
            public string BookingStatus { get; set; }
        }

    }
}
