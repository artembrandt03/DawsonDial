using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels
{
    public partial class StudentMyCoursesViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Section> enrolledCourses = new();

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError = false;

        private readonly Student _student;

        public StudentMyCoursesViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            Initialize(student, service, mainWindow);
            _student = student;
            LoadEnrolledCourses();
        }

        private void LoadEnrolledCourses()
        {
            try
            {
                var courses = _student.Classes;
                EnrolledCourses = new ObservableCollection<Section>(courses);
                
                if (courses.Count == 0)
                {
                    ErrorMessage = "You are not enrolled in any courses.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading courses: {ex.Message}";
                HasError = true;
            }
        }
    }
}