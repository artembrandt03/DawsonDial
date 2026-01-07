using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public class CourseMenuViewModel : ViewModelBase
{
    public ICommand ViewAllCoursesCommand { get; }
    public ICommand AddCourseCommand { get; }
    public ICommand GoBackToMenuCommand { get; }

    public CourseMenuViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);

        ViewAllCoursesCommand = new RelayCommand(OpenViewAllCourses);
        AddCourseCommand = new RelayCommand(OpenAddCourse);
        GoBackToMenuCommand = new RelayCommand(GoBack);
    }

    private void OpenViewAllCourses()
    {
        SetCurrentView(new ViewAllCoursesView
        {
            DataContext = new ViewAllCoursesViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void OpenAddCourse()
    {
        SetCurrentView(new AddCourseView
        {
            DataContext = new AddCourseViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void GoBack()
    {
        SetCurrentView(new AdminMenuView
        {
            DataContext = new AdminMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}