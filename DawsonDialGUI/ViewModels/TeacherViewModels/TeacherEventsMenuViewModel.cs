using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

/// <summary>
/// ViewModel for the teacher's Events submenu.
/// </summary>
public partial class TeacherEventsMenuViewModel : ViewModelBase
{
    // Commands for events options
    public ICommand ViewMyEventsCommand { get; }
    public ICommand ViewAllEventsCommand { get; }

    /// <summary>
    /// Initializes the Events submenu ViewModel.
    /// </summary>
    /// <param name="teacher">The teacher to display the events menu for.</param>
    /// <param name="service">The service to use for the events menu.</param>
    /// <param name="mainWindow">The main window to use for the events menu.</param>
    public TeacherEventsMenuViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        Initialize(teacher, service, mainWindow);
        ViewMyEventsCommand = new RelayCommand(OpenViewMyEvents);
        ViewAllEventsCommand = new RelayCommand(OpenViewAllEvents);
    }

    // Navigation methods for each option
    /// <summary>
    /// Opens the View My Events view.
    /// </summary>
    private async void OpenViewMyEvents()
    {
        var teacher = (Teacher)LoggedInUser;
        var viewModel = await ViewMyEventsViewModel.CreateAsync(teacher, Service, MainWindow);

        var view = new Views.TeacherViews.ViewMyEventsView
        {
            DataContext = viewModel
        };

        SetCurrentView(view);
    }

    /// <summary>
    /// Opens the View All Events view for registration.
    /// </summary>
    private async void OpenViewAllEvents()
    {
        var teacher = (Teacher)LoggedInUser;
        var viewModel = await ViewAllEventsViewModel.CreateAsync(teacher, Service, MainWindow);

        var view = new Views.TeacherViews.ViewAllEventsView
        {
            DataContext = viewModel
        };

        SetCurrentView(view);
    }
}