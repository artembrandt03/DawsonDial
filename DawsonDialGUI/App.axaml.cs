using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using DawsonDialGUI.ViewModels;
using DawsonDialGUI.Views;
using DawsonDial.Services.ManagerService;
using DawsonDial.Models.Contexts;
using DawsonDial.Repositories;

namespace DawsonDialGUI;

public partial class App : Application
{
        private DawsonDialContext _context = new DawsonDialContext();
        private DawsonDialService? _service;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Initialize the repositories
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

            var mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel(_service, mainWindow);
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
