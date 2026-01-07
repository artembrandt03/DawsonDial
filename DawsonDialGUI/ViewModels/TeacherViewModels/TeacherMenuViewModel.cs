using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

public partial class TeacherMenuViewModel : ViewModelBase
{
    // Make teacher public for potential binding usage
    public Teacher Teacher => (Teacher)LoggedInUser;

    // Menu Commands
    public ICommand ViewTeacherProfileCommand { get; }
    public ICommand ViewMySectionsCommand { get; }
    public ICommand SchedulingCommand { get; }
    public ICommand EventsCommand { get; }

    /// <summary>
    /// Initializes the TeacherMenuViewModel with the provided teacher, service, and main window.
    /// </summary>
    /// <param name="teacher">The Teacher viewing the menu.</param>
    /// <param name="service">The DawsonDialService manager.</param>
    /// <param name="mainWindow">The main avalonia window.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
    public TeacherMenuViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        if (teacher == null) throw new ArgumentNullException(nameof(teacher));
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));

        Initialize(teacher, service, mainWindow);

        // Main menu navigation commands
        ViewTeacherProfileCommand = new RelayCommand(() => ViewProfile(Teacher));
        ViewMySectionsCommand = new RelayCommand(ViewMySections);
        SchedulingCommand = new RelayCommand(OpenScheduling);
        EventsCommand = new RelayCommand(OpenEvents);
    }

    /// <summary>
    /// Navigates to the teacher's courses/sections view.
    /// </summary>
    private void ViewMySections()
    {
        var view = new Views.TeacherViews.TeacherSectionsView
        {
            DataContext = new TeacherSectionsViewModel(Teacher, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    /// <summary>
    /// Navigates to the Scheduling/Availability menu for the teacher.
    /// </summary>
    private void OpenScheduling()
    {
        var view = new Views.TeacherViews.TeacherSchedulingMenuView
        {
            DataContext = new TeacherSchedulingMenuViewModel(Teacher, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    /// <summary>
    /// Navigates to the Events menu for the teacher.
    /// </summary>
    private void OpenEvents()
    {
        var view = new Views.TeacherViews.TeacherEventsMenuView
        {
            DataContext = new TeacherEventsMenuViewModel(Teacher, Service, MainWindow)
        };
        SetCurrentView(view);
    }
}
