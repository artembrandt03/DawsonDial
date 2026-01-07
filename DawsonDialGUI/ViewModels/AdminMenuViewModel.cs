using System;
using System.Windows.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class AdminMenuViewModel : ViewModelBase
{
    // Make admin public for potential binding usage
    public Admin Admin => (Admin)LoggedInUser;

    // Admin Menu Commands
    public ICommand ViewAdminProfileCommand { get; }
    public ICommand ViewLogsCommand { get; }
    public ICommand UserMenuCommand { get; }
    public ICommand RoomMenuCommand { get; }
    public ICommand EventMenuCommand { get; }
    public ICommand CourseMenuCommand { get; }

    /// <summary>
    /// Initializes the AdminMenuViewModel with the provided admin, service, and main window.
    /// </summary>
    /// <param name="admin">The Admin viewing the menu.</param>
    /// <param name="service">The DawsonDialService manager.</param>
    /// <param name="mainWindow">The main avalonia window.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
    public AdminMenuViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        if (admin == null) throw new ArgumentNullException(nameof(admin));
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));

        Initialize(admin, service, mainWindow);

        ViewAdminProfileCommand = new RelayCommand(() => ViewProfile(Admin));
        ViewLogsCommand = new RelayCommand(OpenViewLogs);
        UserMenuCommand = new RelayCommand(OpenUserMenu);
        RoomMenuCommand = new RelayCommand(OpenRoomMenu);
        EventMenuCommand = new RelayCommand(OpenEventMenu);
        CourseMenuCommand = new RelayCommand(OpenCourseMenu);
    }

    private void OpenViewLogs()
    {
        SetCurrentView(new ViewLogsView
        {
            DataContext = new AdminLogsViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void OpenUserMenu()
    {
        SetCurrentView(new UserMenuView
        {
            DataContext = new UserMenuViewModel(Admin, Service, MainWindow)
        });
    }

    private void OpenRoomMenu()
    {
        SetCurrentView(new RoomMenuView
        {
            DataContext = new RoomMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void OpenEventMenu()
    {
        SetCurrentView(new EventMenuView
        {
            DataContext = new EventMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void OpenCourseMenu()
    {
        SetCurrentView(new CourseMenuView
        {
            DataContext = new CourseMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}
