using System.Threading.Tasks;
using DawsonDial.Models.People;
using DawsonDial.Repositories.Interfaces;
using DawsonDial.Models.Events;

namespace DawsonDial.Services.ManagerService
{
    /// <summary>
    /// Provides user management services.
    /// </summary>
    public class UserService
    {
        // UserService backing fields
        private readonly IPersonRepository _personRepository;
        // UserService properties
        public IPersonRepository PersonRepository => _personRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        public UserService(IPersonRepository personRepository)
        {
            // Initialize the user repository
            _personRepository = personRepository;
        }

        /// <summary>
        /// Adds a user to the system.
        /// </summary>
        /// <param name="person">The user to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when person is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the username already exists.</exception>
        public async Task AddUser(Person person)
        {
            ArgumentNullException.ThrowIfNull(person);

            if (await _personRepository.GetUserByUsernameAsync(person.Username) != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }
            await _personRepository.AddAsync(person);
        }

        /// <summary>
        /// Updates a user in the system.
        /// </summary>
        /// <param name="person">The user to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when person is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the user does not exist.</exception>
        public async Task UpdateUser(Person person)
        {
            ArgumentNullException.ThrowIfNull(person);

            var existingUser = await GetUserByUsername(person.Username) ?? throw new ArgumentException("User not found.");
            await _personRepository.UpdateAsync(person);
        }

        /// <summary>
        /// Gets all users in the system.
        /// </summary>
        /// <returns>All the users in the database</returns>
        public async Task<IEnumerable<Person>> GetAllUsers()
        {
            return await _personRepository.GetAllAsync();
        }

        /// <summary>
        /// Removes a user from the system.
        /// </summary>
        /// <param name="person">The user to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown when person is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the user does not exist.</exception>
        public async Task DeleteUser(Person person)
        {
            ArgumentNullException.ThrowIfNull(person);

            if (await _personRepository.GetUserByUsernameAsync(person.Username) == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }
            await _personRepository.DeleteAsync(person.PersonId);
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>True if the user is authenticated, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when username or password is null.</exception>
        public async Task<bool> Authenticate(string username, string password)
        {
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(password);

            return await _personRepository.GetUserByUsernameAndPasswordAsync(username, password) != null;
        }

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user with the specified username.</returns>
        /// <exception cref="ArgumentNullException">Thrown when username is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when user could not be found.</exception>
        public async Task<Person> GetUserByUsername(string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            return await _personRepository.GetUserByUsernameAsync(username) ?? throw new ArgumentNullException("Could not find user");
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="personId">The ID of the user.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown when personId is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when user could not be found.</exception>
        public async Task<Person> GetUserById(Guid personId)
        {
            ArgumentNullException.ThrowIfNull(personId);

            return await _personRepository.GetByIdAsync(personId) ?? throw new ArgumentNullException("Could not find user");
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="oldPassword">The old password of the user.</param>
        /// <param name="newPassword">The new password of the user.</param>
        /// <exception cref="ArgumentNullException">Thrown when username, oldPassword, or newPassword is null.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user does not exist or the old password is incorrect.</exception>
        public async Task UpdatePassword(string username, string oldPassword, string newPassword)
        {
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(oldPassword);
            ArgumentNullException.ThrowIfNull(newPassword);

            if (await _personRepository.GetUserByUsernameAndPasswordAsync(username, oldPassword) != null)
            {
                await _personRepository.UpdateUserPasswordByUsernameAsync(username, newPassword);
            }
            else
            {
                throw new UnauthorizedAccessException("Credentials are incorrect.");
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //For GUI app:
        public async Task CreateStudent(string username, string password, string firstName, string lastName, int age, int studentId, string program, int year)
        {
            var student = new Student(
                username,
                password,
                firstName,
                lastName,
                age,
                isDisabled: false,
                description: "",
                studentId,
                program,
                year,
                new HashSet<Section>());

            await AddUser(student);
        }

        public async Task CreateTeacher(string username, string password, string firstName, string lastName, int age, string department)
        {
            var teacher = new Teacher(
                username,
                password,
                firstName,
                lastName,
                age,
                isDisabled: false,
                description: "",
                department,
                new HashSet<Section>(),
                officeRoom: null!,
                officeHours: null!);

            await AddUser(teacher);
        }

        public async Task CreateAdmin(string username, string password, string firstName, string lastName, int age)
        {
            var admin = new Admin(
                username,
                password,
                firstName,
                lastName,
                age,
                isDisabled: false,
                description: "");

            await AddUser(admin);
        }
    }
}
