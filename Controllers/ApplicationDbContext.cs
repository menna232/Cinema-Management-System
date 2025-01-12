using Cinema_Management_System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Management_System.Controllers
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString= "Server=DESKTOP-382NF2R\\SQLEXPRESS;Database=CinemaManagement;Trusted_Connection=True;TrustServerCertificate=True;";
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        // Parameterless constructor for migrations
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<ShowTime> Showtimes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        

    }
}
