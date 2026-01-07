using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels
{
    public partial class StudentEnrollViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Course> availableCourses = new();

        [ObservableProperty]
        private Course? selectedCourse;

        [ObservableProperty]
        private Section? selectedSection;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError = false;

        private readonly Student _student;

        public StudentEnrollViewModel(Student student, DawsonDialService service, Window mainWindow)
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
                    ErrorMessage = "No courses available for enrollment.";
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
        private async Task EnrollInCourse()
        {
            if (SelectedCourse == null || SelectedSection == null)
            {
                ErrorMessage = "Please select a course and section.";
                HasError = true;
                return;
            }

            try
            {
                await Service.EnrollStudentInCourse(_student, SelectedSection);
                NavigateToMyCourses();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error enrolling in course: {ex.Message}";
                HasError = true;
            }
        }

        [RelayCommand]
        private void NavigateToMyCourses()
        {
            var myCoursesView = new StudentMyCoursesView
            {
                DataContext = new StudentMyCoursesViewModel(_student, Service, MainWindow)
            };
            SetCurrentView(myCoursesView);
        }

        [RelayCommand]
        private void GoBack()
        {
            var availableCoursesView = new StudentViewAvailableCoursesView
            {
                DataContext = new StudentViewAvailableCoursesViewModel(_student, Service, MainWindow)
            };
            SetCurrentView(availableCoursesView);
        }
    }
}