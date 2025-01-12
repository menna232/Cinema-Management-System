using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_System.Data
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public List<int> SeatNumbers { get; set; } = new List<int>();
        public DateTime ReservationDate { get; set; }
        public List<ShowTime> Showtimes { get; set; } = new List<ShowTime>();


    }
}
