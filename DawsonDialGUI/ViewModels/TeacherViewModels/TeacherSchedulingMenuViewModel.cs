using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using Avalonia.Threading;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

/// <summary>
/// ViewModel for the teacher's Scheduling submenu.
/// </summary>
public partial class TeacherSchedulingMenuViewModel : ViewModelBase
{
    // Commands for scheduling options
    public ICommand SetOfficeHoursCommand { get; }
    public ICommand ViewMyScheduleCommand { get; }
    public ICommand ScheduleConferenceCommand { get; }
    public ICommand ViewRoomAvailabilityCommand { get; }

    /// <summary>
    /// Initializes the Scheduling submenu ViewModel.
    /// </summary>
    /// <param name="teacher">The teacher to display the scheduling menu for.</param>
    /// <param name="service">The service to use for the scheduling menu.</param>
    /// <param name="mainWindow">The main window to use for the scheduling menu.</param>
    public TeacherSchedulingMenuViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        Initialize(teacher, service, mainWindow);
        SetOfficeHoursCommand = new RelayCommand(OpenSetOfficeHours);
        ViewMyScheduleCommand = new RelayCommand(OpenViewMySchedule);
        ScheduleConferenceCommand = new RelayCommand(OpenScheduleConference);
        ViewRoomAvailabilityCommand = new RelayCommand(OpenViewRoomAvailability);
    }

    // Navigation methods for each option
    /// <summary>
    /// Opens the Set Office Hours view.
    /// </summary>
    private void OpenSetOfficeHours()
    {
        var view = new Views.TeacherViews.SetOfficeHoursView
        {
            DataContext = new SetOfficeHoursViewModel((Teacher)LoggedInUser, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    /// <summary>
    /// Opens the View My Schedule view.
    /// </summary>
    private async void OpenViewMySchedule()
    {
        var view = new Views.TeacherViews.ViewMyScheduleView();
        var vm = await ViewMyScheduleViewModel.CreateAsync((Teacher)LoggedInUser, Service, MainWindow);
        // Set DataContext on UI thread
        Dispatcher.UIThread.Post(() => view.DataContext = vm);
        SetCurrentView(view);
    }

    /// <summary>
    /// Opens the Schedule Conference view.
    /// </summary>
    private void OpenScheduleConference()
    {
        var view = new Views.TeacherViews.ConferenceScheduleView
        {
            DataContext = new ConferenceScheduleViewModel((Teacher)LoggedInUser, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    /// <summary>
    /// Opens the View Room Availability view.
    /// </summary>
    private void OpenViewRoomAvailability()
    {
        var view = new Views.TeacherViews.ViewRoomAvailabilityView
        {
            DataContext = new ViewRoomAvailabilityViewModel((Teacher)LoggedInUser, Service, MainWindow)
        };
        SetCurrentView(view);
    }
} 