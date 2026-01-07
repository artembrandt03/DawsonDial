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
    public partial class StudentViewAvailableCoursesViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Course> availableCourses = new();

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError = false;

        private readonly Student _student;

        public StudentViewAvailableCoursesViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            Initialize(student, service, mainWindow);
            _student = student;
            LoadAvailableCoursesAsync();
        }

        private async void LoadAvailableCoursesAsync()
        {
            try
            {
                var courses = await Service.GetAvailableCourses();
                AvailableCourses = new ObservableCollection<Course>(courses);
                
                if (courses.Count == 0)
                {
                    ErrorMessage = "No courses available at this time.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading courses: {ex.Message}";
                HasError = true;
            }
        }

        [RelayCommand]
        private void NavigateToEnroll()
        {
            var enrollView = new StudentEnrollView
            {
                DataContext = new StudentEnrollViewModel(_student, Service, MainWindow)
            };
            SetCurrentView(enrollView);
        }
    }
}