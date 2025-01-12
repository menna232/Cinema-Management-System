using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_System.Data
{
    public  class ShowTime
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public DateTime Date { get; set; }
        public int TotalSeats { get; set; }
        public List<int> ReservedSeats { get; set; } = new List<int>();
        public List<Movie> Movie { get; set; } = new List<Movie>();

    }
}
