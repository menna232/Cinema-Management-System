using Cinema_Management_System.Controllers;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_System.Data
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int age { get; set; }
        public string PhoneNumber {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public void SignUp(ApplicationDbContext context)
        {
            Console.WriteLine("Enter your name:");
            string name = Console.ReadLine();

            Console.WriteLine("Enter your age:");
            int age = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter your phone number:");
            string phoneNumber = Console.ReadLine();

            Console.WriteLine("Enter your email:");
            string email = Console.ReadLine();

            // Check if user already exists by email
            if (context.Users.Any(u => u.Email == email))
            {
                Console.WriteLine("A user with this email already exists.");
                return;
            }

            // Creating a new user instance
            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();



            var newUser = new User
            {
                Name = name,
                age = age,
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password
            };

            context.Users.Add(newUser);
            context.SaveChanges();

            Console.WriteLine("User signed up successfully!");
        }
        public void logIn(ApplicationDbContext context)
        {
            Console.WriteLine("Enter User Name:");
            string Name = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            string Password = Console.ReadLine();

            var dbUser = context.Users.FirstOrDefault(a => a.Name == Name);

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
        public void BrowseMovies(ApplicationDbContext context)
        {
            var movies = context.Movies.Include(m => m.Showtimes).ToList(); // Include showtimes when retrieving movies

            if (movies.Count == 0)
            {
                Console.WriteLine("There are no available movies right now.");
            }
            else
            {
                Console.WriteLine("All Movies:");
                foreach (var movie in movies)
                {
                    Console.WriteLine($"Movie ID: {movie.MovieId}");
                    Console.WriteLine($"Movie Name: {movie.Title}");
                    Console.WriteLine($"Movie Description: {movie.Description}");
                    Console.WriteLine($"Movie Type: {movie.Type}");

                    if (movie.Showtimes.Count == 0)
                    {
                        Console.WriteLine("No showtimes available for this movie.");
                    }
                    else
                    {
                        Console.WriteLine("Showtimes:");
                        foreach (var showtime in movie.Showtimes)
                        {
                            Console.WriteLine($" - Showtime ID: {showtime.ShowtimeId}");
                            Console.WriteLine($"   Date and Time: {showtime.Date}");
                            Console.WriteLine($"   Total Seats: {showtime.TotalSeats}");
                            Console.WriteLine($"   Reserved Seats: {string.Join(", ", showtime.ReservedSeats)}");
                        }
                    }

                    Console.WriteLine(new string('-', 50)); // Separator for readability
                }
            }
        }

        public void ReserveSeat(ApplicationDbContext context)
        {
            Console.WriteLine("Enter the movie title you want to reserve a seat for:");
            string movieTitle = Console.ReadLine();

         
            var movie = context.Movies
                               .Include(m => m.Showtimes)
                               .FirstOrDefault(m => m.Title.ToLower() == movieTitle);
            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            // Display available showtimes
            if (movie.Showtimes.Count == 0)
            {
                Console.WriteLine("No showtimes available for this movie.");
                return;
            }

            Console.WriteLine("Available Showtimes:");
            foreach (var showtime in movie.Showtimes)
            {
                Console.WriteLine($"Showtime ID: {showtime.ShowtimeId}");
                Console.WriteLine($"Date and Time: {showtime.Date}");
                Console.WriteLine($"Available Seats: {showtime.TotalSeats - showtime.ReservedSeats.Count}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("Enter the Showtime ID for your reservation:");
            int showtimeId;
            while (!int.TryParse(Console.ReadLine(), out showtimeId) || !movie.Showtimes.Any(s => s.ShowtimeId == showtimeId))
            {
                Console.WriteLine("Invalid Showtime ID. Please try again:");
            }

            // Find the selected showtime
            var selectedShowtime = movie.Showtimes.First(s => s.ShowtimeId == showtimeId);

            Console.WriteLine($"Enter the seat number(s) you want to reserve (comma-separated):");
            var seatInput = Console.ReadLine();
            var seatNumbers = seatInput.Split(',').Select(s => int.TryParse(s.Trim(), out var seat) ? seat : -1).ToList();

            // Validate and reserve seats
            foreach (var seat in seatNumbers)
            {
                if (seat <= 0 || seat > selectedShowtime.TotalSeats)
                {
                    Console.WriteLine($"Seat {seat} is invalid.");
                }
                else if (selectedShowtime.ReservedSeats.Contains(seat))
                {
                    Console.WriteLine($"Seat {seat} is already reserved.");
                }
                else
                {
                    selectedShowtime.ReservedSeats.Add(seat);
                    Console.WriteLine($"Seat {seat} reserved successfully.");
                }
            }

            // Save the reservation to the database
            var reservation = new Reservation
            {
                UserId = 1, // Assuming user is logged in and user ID is 1
                ShowtimeId = selectedShowtime.ShowtimeId,
                SeatNumbers = seatNumbers.Where(s => s > 0 && !selectedShowtime.ReservedSeats.Contains(s)).ToList(),
                ReservationDate = DateTime.Now
            };

            context.Reservations.Add(reservation);
            context.SaveChanges();

            Console.WriteLine("Reservation completed successfully!");
        }

        public void CancelReservation(ApplicationDbContext context)
        {
            Console.WriteLine("Enter the Reservation ID you want to cancel:");
            int reservationId;
            while (!int.TryParse(Console.ReadLine(), out reservationId))
            {
                Console.WriteLine("Invalid input. Please enter a valid Reservation ID.");
            }

            // Assuming the user is logged in with a specific UserId
            int userId = 2; // You would normally get the logged-in user's ID here

            var reservation = context.Reservations
                                     .FirstOrDefault(r => r.ReservationId == reservationId && r.UserId == userId);

            if (reservation == null)
            {
                Console.WriteLine("Reservation not found or you are not authorized to cancel this reservation.");
                return;
            }

            // Find the associated Showtime and update the reserved seats
            var showtime = context.Showtimes.FirstOrDefault(s => s.ShowtimeId == reservation.ShowtimeId);

            if (showtime != null)
            {
                // Remove the reserved seats from the showtime
                foreach (var seat in reservation.SeatNumbers)
                {
                    showtime.ReservedSeats.Remove(seat);
                }

                // Save the changes to the database
                context.Reservations.Remove(reservation);
                context.SaveChanges();
                Console.WriteLine("Reservation cancelled successfully.");
            }
            else
            {
                Console.WriteLine("Error: Showtime not found.");
            }
        }

        public void ViewMyReservations(ApplicationDbContext context)
        {
            // Assuming the user is logged in with a specific UserId
            int userId = 1; // You would normally get the logged-in user's ID here

            var reservations = context.Reservations
                                      .Where(r => r.UserId == userId).ToList();

            if (reservations.Count == 0)
            {
                Console.WriteLine("You have no reservations.");
                return;
            }

            Console.WriteLine("Your Reservations:");
            foreach (var reservation in reservations)
            {
                Console.WriteLine($"Reservation ID: {reservation.ReservationId}");
                Console.WriteLine($"Reserved Seats: {string.Join(", ", reservation.SeatNumbers)}");
                Console.WriteLine($"Reservation Date: {reservation.ReservationDate}");
                Console.WriteLine(new string('-', 30));
            }
        }


    }
}
