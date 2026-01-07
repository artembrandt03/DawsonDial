using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Models;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public class ViewAllCoursesViewModel : ViewModelBase
{
    public ObservableCollection<CourseDisplay> Courses { get; } = new();

    public ICommand GoBackToMenuCommand { get; }

    public ViewAllCoursesViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        GoBackToMenuCommand = new RelayCommand(GoBackToMenu);
        _ = LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        var allCourses = await Service.GetAllCoursesAsync();
        Courses.Clear();

        foreach (var course in allCourses)
        {
            Courses.Add(new CourseDisplay(course));
        }
    }
    public void OnModifyCourseClick(Course course)
    {
        SetCurrentView(new ModifyCourseView
        {
            DataContext = new ModifyCourseViewModel((Admin)LoggedInUser, Service, MainWindow, course)
        });
    }

    private void GoBackToMenu()
    {
        SetCurrentView(new CourseMenuView
        {
            DataContext = new CourseMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    public class CourseDisplay
    {
        public Course OriginalCourse { get; }

        public string CourseCode => OriginalCourse.CourseCode;
        public string Subject => OriginalCourse.Subject;

        public CourseDisplay(Course course)
        {
            OriginalCourse = course;
        }
    }
}