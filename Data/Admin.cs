using Cinema_Management_System.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_System.Data
{
    [Table("Admins")]
    public class Admin
    {
        public int AdminId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
      
        [Column(TypeName = "nvarchar(15)")]
        public string Password { get; set; }
        
       
        public void logIn(ApplicationDbContext context)
        {
            Console.WriteLine("Enter User Name:");
            string Name = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            string Password = Console.ReadLine();

            var dbUser = context.Admins.FirstOrDefault(a => a.Name == Name && a.Password == Password);

            if (dbUser == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            if (dbUser.Password == Password)
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Incorrect password.");
            }

        }

        public void AddMovie(ApplicationDbContext context)
        {
            Console.WriteLine("Enter movie title:");
            string title = Console.ReadLine();

            Console.WriteLine("Enter movie description:");
            string description = Console.ReadLine();

            Console.WriteLine("Enter movie type:");
            string type = Console.ReadLine();

            Console.WriteLine("Enter release date (yyyy-mm-dd):");
            DateTime releaseDate;
            while (!DateTime.TryParse(Console.ReadLine(), out releaseDate))
            {
                Console.WriteLine("Invalid date format. Please try again:");
            }

            var newMovie = new Movie
            {
                Title = title,
                Description = description,
                Type = type,
            };

            // Add showtimes
            bool addMoreShowtimes = true;
            while (addMoreShowtimes)
            {
                Console.WriteLine("Enter showtime date and time (yyyy-mm-dd hh:mm):");
                DateTime showtimeDate;
                while (!DateTime.TryParse(Console.ReadLine(), out showtimeDate))
                {
                    Console.WriteLine("Invalid date and time format. Please try again:");
                }

                Console.WriteLine("Enter total seats for this showtime:");
                int totalSeats;
                while (!int.TryParse(Console.ReadLine(), out totalSeats) || totalSeats <= 0)
                {
                    Console.WriteLine("Invalid number of seats. Please enter a positive number:");
                }

                var showtime = new ShowTime
                {
                    Date = showtimeDate,
                    TotalSeats = totalSeats
                };

                newMovie.Showtimes.Add(showtime);

                Console.WriteLine("Do you want to add another showtime? (yes/no)");
                string response = Console.ReadLine().ToLower();
                addMoreShowtimes = response == "yes";
            }

            // Save the movie and its showtimes to the database
            context.Movies.Add(newMovie);
            context.SaveChanges();

            Console.WriteLine("Movie and showtimes added successfully!");
        }

        public void DeleteMovie(ApplicationDbContext context) 
        {
            Console.WriteLine("Enter The movie you want to delete  : ");
            string name = Console.ReadLine();

            var DbMovie = context.Movies.FirstOrDefault(a => a.Title == name);

            if (DbMovie == null) {
                Console.WriteLine("Movie not found");
            }

            else
            {
                context.Movies.Remove(DbMovie);
                context.SaveChanges();
                Console.WriteLine("Movie Deleted successfully!");

            }
        }

        public void ViewAllReservations(ApplicationDbContext context)
        {
            var reservations = context.Reservations.ToList();

            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }

            Console.WriteLine("All Reservations:");
            foreach (var reservation in reservations)
            {
                Console.WriteLine($"Reservation ID: {reservation.ReservationId}");
                Console.WriteLine($"User ID: {reservation.UserId}");
                Console.WriteLine($"Showtime ID: {reservation.ShowtimeId}");
                Console.WriteLine($"Reserved Seats: {string.Join(", ", reservation.SeatNumbers)}");
                Console.WriteLine($"Reservation Date: {reservation.ReservationDate}");
                Console.WriteLine(new string('-', 30)); // Separator for readability
            }
        }



    }
}
