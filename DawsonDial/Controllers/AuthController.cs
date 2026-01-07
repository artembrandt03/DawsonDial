using DawsonDial.Helpers;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;

namespace DawsonDial.Controllers
{
    /// <summary>
    /// Handles authentication-related operations.
    /// </summary>
    public class AuthController
    {
        private readonly DawsonDialService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="service">The service instance.</param>
        public AuthController(DawsonDialService service)
        {
            _service = service;
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <returns>The logged-in user or null if login failed.</returns>
        public async Task<Person?> LoginAsync()
        {
            Console.WriteLine("\n===== Login =====");

            string username = InputPrompter.Prompt("Username: ");
            string password = InputPrompter.PromptPassword("Password: ");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username and password cannot be empty.");
                return null;
            }

            try
            {
                bool isAuthenticated = await _service.AuthenticateUser(username, password);

                if (isAuthenticated)
                {
                    var user = await _service.GetUserByUsername(username);
                    Console.WriteLine($"Welcome {user.FirstName} {user.LastName}!");
                    return user;
                }
                else
                {
                    Console.WriteLine("Invalid username or password.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred during login: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Logs out a user.
        /// </summary>
        /// <param name="user">The user to log out.</param>
        public void Logout(Person user)
        {
            Console.WriteLine($"\nLogging out user: {user.FirstName} {user.LastName}.");
            Console.WriteLine("\nUser logged out successfully.");
            Console.WriteLine("Thank you for using Dawson Dial!");
        }
    }
}
