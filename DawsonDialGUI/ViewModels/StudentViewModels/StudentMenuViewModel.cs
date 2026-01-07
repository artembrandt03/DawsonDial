using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using Avalonia.Threading;
using DawsonDialGUI.Views;
using DawsonDialGUI.ViewModels.StudentViewModels;

namespace DawsonDialGUI.ViewModels;

public partial class StudentMenuViewModel : ViewModelBase
{
    private readonly Student _student;

    // Menu Commands
    public RelayCommand ViewStudentProfileCommand { get; }
    public RelayCommand ViewAvailableCoursesCommand { get; }
    public RelayCommand ViewMyCoursesCommand { get; }
    public RelayCommand EnrollInCourseCommand { get; }
    public RelayCommand DropCourseCommand { get; }
    public RelayCommand ViewAvailableEventsCommand { get; }
    public RelayCommand BookTeacherMeetingCommand { get; }
    public RelayCommand ViewMyTeacherMeetingsCommand { get; }
    public RelayCommand ViewMyEventsCommand { get; }
    public RelayCommand ViewMyScheduleCommand { get; }
    
    /// <summary>
    /// Initializes the StudentMenuViewModel with the provided student, service, and main window.
    /// </summary>
    /// <param name="student">The Student viewing the menu.</param>
    /// <param name="service">The DawsonDialService manager.</param>
    /// <param name="mainWindow">The main avalonia window.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null.</exception>
    public StudentMenuViewModel(Student student, DawsonDialService service, Window mainWindow)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));

        Initialize(student, service, mainWindow);
        _student = student;

        ViewStudentProfileCommand = new RelayCommand(() => ViewProfile(_student));
        ViewAvailableCoursesCommand = new RelayCommand(ViewAvailableCourses);
        ViewMyCoursesCommand = new RelayCommand(ViewMyCourses);
        EnrollInCourseCommand = new RelayCommand(EnrollInCourse);
        DropCourseCommand = new RelayCommand(DropCourse);
        ViewAvailableEventsCommand = new RelayCommand(ViewAvailableEvents);
        BookTeacherMeetingCommand = new RelayCommand(BookTeacherMeeting);
        ViewMyTeacherMeetingsCommand = new RelayCommand(ViewMyTeacherMeetings);
        ViewMyEventsCommand = new RelayCommand(ViewMyEvents);
        ViewMyScheduleCommand = new RelayCommand(ViewMySchedule);
    }

    private void ViewAvailableCourses()
    {
        var view = new StudentViewAvailableCoursesView
        {
            DataContext = new StudentViewAvailableCoursesViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    private void ViewMyCourses()
    {
        var view = new StudentMyCoursesView
        {
            DataContext = new StudentMyCoursesViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    private void EnrollInCourse()
    {
        var view = new StudentEnrollView
        {
            DataContext = new StudentEnrollViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    private void DropCourse()
    {
        var view = new StudentDropCourseView
        {
            DataContext = new StudentDropCourseViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    private void ViewAvailableEvents()
    {
        var view = new StudentViewAvailableEventsView
        {
            DataContext = new StudentViewAvailableEventsViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }

    private void BookTeacherMeeting()
    {
        var view = new Views.StudentViews.BookTeacherMeetingView
        {
            DataContext = new BookTeacherMeetingViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }    private void ViewMyTeacherMeetings()
    {
        var view = new Views.StudentViews.StudentTeacherMeetingsView
        {
            DataContext = new StudentTeacherMeetingsViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }private void ViewMyEvents()
    {
        var view = new Views.StudentViews.StudentViewMyEventsView
        {
            DataContext = new StudentViewMyEventsViewModel(_student, Service, MainWindow)
        };
        SetCurrentView(view);
    }    private async void ViewMySchedule()
    {
        var view = new Views.StudentViews.ViewMyScheduleView();
        var vm = await ViewMyScheduleViewModel.CreateAsync(_student, Service, MainWindow);
        // Set DataContext on UI thread
        Dispatcher.UIThread.Post(() => view.DataContext = vm);
        SetCurrentView(view);
    }
}
