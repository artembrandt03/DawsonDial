using DawsonDial.Models.People;
using DawsonDial.Models.Contexts;
using DawsonDial.Controllers;
using DawsonDial.Repositories;
using DawsonDial.Views;
using DawsonDial.Services.ManagerService;

namespace DawsonDial
{
    /// <summary>
    /// Represents the main application class for DawsonDial.
    /// </summary>
    public class DawsonDialApplication
    {
        private readonly DawsonDialContext _context = new DawsonDialContext();
        private readonly DawsonDialService _service;
        private Person? _currentUser;

        private readonly AuthController _authController;
        private readonly AdminController _adminController;
        private readonly StudentController _studentController;
        private readonly TeacherController _teacherController;

        /// <summary>
        /// Initializes a new instance of the <see cref="DawsonDialApplication"/> class.
        /// </summary>
        public DawsonDialApplication()
        {
            var userRepository = new PersonDbRepository(_context);
            var teacherRepository = new PersonDbRepository(_context);
            var studentRepository = new PersonDbRepository(_context);
            var adminRepository = new PersonDbRepository(_context);
            var adminLogRepositoy = new AdminLogDbRepository(_context);
            var roomRepository = new RoomDbRepository(_context);
            var eventRepository = new EventDbRepository(_context);
            var courseRepository = new CourseDbRepository(_context);
            var sectionRepository = new SectionDbRepository(_context);
            var scheduleRepository = new ScheduleDbRepository(_context);

            // Initialize the services
            var userService = new UserService(userRepository);
            var teacherService = new TeacherService(teacherRepository);
            var studentService = new StudentService(studentRepository);
            var adminService = new AdminService(adminRepository, adminLogRepositoy);
            var roomService = new RoomService(roomRepository, eventRepository);
            var eventService = new EventService(eventRepository);
            var courseService = new CourseService(courseRepository, sectionRepository);
            var scheduleService = new ScheduleService(scheduleRepository);

            // Initialize the DawsonDialService with the services
            _service = new DawsonDialService(
                userService,
                adminService,
                teacherService,
                studentService,
                roomService,
                eventService,
                courseService,
                scheduleService);

            _authController = new AuthController(_service);
            _adminController = new AdminController(_service);
            _studentController = new StudentController(_service);
            _teacherController = new TeacherController(_service);
        }

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static async Task Main(string[] args)
        {
            try
            {
                var app = new DawsonDialApplication();
                await app.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}");
            }
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public async Task Run()
        {
            bool exit = false;

            Console.WriteLine("Welcome to DawsonDial - College Management System");
            Console.WriteLine("=================================================");

            while (!exit)
            {
                if (_currentUser == null)
                {
                    await ShowLoginMenu();
                }
                else
                {
                    await ShowMainMenu();
                }
            }
        }

        /// <summary>
        /// Displays the login menu.
        /// </summary>
        private async Task ShowLoginMenu()
        {
            Console.WriteLine("\n===== Login Menu =====");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Exit");
            Console.WriteLine("Select an option:");
            Console.Write("> ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    _currentUser = await _authController.LoginAsync();
                    break;
                case "2":
                    Console.WriteLine("\nThank you for using DawsonDial!");
                    Environment.Exit(0); // ends app immediately
                    break;
                default:
                    Console.WriteLine("\nInvalid option. Please try again.");
                    break;
            }
        }

        /// <summary>
        /// Displays the main menu depending on the user's type.
        /// </summary>
        private async Task ShowMainMenu()
        {
            if (_currentUser == null) return;

            if (_currentUser is Student student)
            {
                await StudentViews.ShowAsync(student, _studentController, _authController);
                _currentUser = null;
            }

            if (_currentUser is Teacher teacher)
            {
                await TeacherViews.ShowAsync(teacher, _teacherController, _authController);
                _currentUser = null;
            }
            if (_currentUser is Admin admin)
            {
                await AdminViews.ShowAsync(admin, _adminController, _authController);
                _currentUser = null;
            }
        }
    }
}
