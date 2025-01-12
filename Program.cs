using Cinema_Management_System.Controllers;
using Cinema_Management_System.Data;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        // Retrieve connection string
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Connection String: {connectionString}");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        // Step 4: Instantiate ApplicationDbContext
        using var context = new ApplicationDbContext(optionsBuilder.Options);
        Admin admin = new Admin();
        User user = new User();

        // Step 3: Ask the user if they want to log in as an Admin or User
        Console.WriteLine("Welcome to the Cinema Management System!");
        Console.WriteLine("Please select your role:");
        Console.WriteLine("1. Admin");
        Console.WriteLine("2. User");
        string roleSelection = Console.ReadLine();

        // Step 4: Role-based logic
        if (roleSelection == "1")
        {
            // Admin login
            Console.WriteLine("You selected Admin.");
            admin.logIn(context);
        }
        else if (roleSelection == "2")
        {
            // User login
            Console.WriteLine("You selected User.");
            Console.WriteLine("Do You Want To LogIn or SighUp");
            int selectio2 =Convert.ToInt32(Console.ReadLine());
            if (selectio2 == 1)
            {
                user.logIn(context);
            }
            else if (selectio2 == 2)
            {
                user.SignUp(context);
            }
            else
            {
                Console.WriteLine("wrong selection");
            }
        }
        else
        {
            Console.WriteLine("Invalid selection. Exiting...");
        }
    }

    // Admin login logic
   

}