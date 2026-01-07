using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;

namespace DawsonDialGUI.ViewModels
{
    public partial class StudentDropCourseViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Section> enrolledCourses = new();

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError = false;

        private readonly Student _student;

        public StudentDropCourseViewModel(Student student, DawsonDialService service, Window mainWindow)
        {
            Initialize(student, service, mainWindow);
            _student = student;
            LoadEnrolledCourses();
            DropCourseCommand = new AsyncRelayCommand<Section>(DropCourseAsync);
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

        public AsyncRelayCommand<Section> DropCourseCommand { get; }

        private async Task DropCourseAsync(Section? section)
        {
            if (section == null) return;

            try
            {
                await Service.DropStudentFromCourse(_student, section);
                EnrolledCourses.Remove(section);
                
                if (EnrolledCourses.Count == 0)
                {
                    ErrorMessage = "You are not enrolled in any courses.";
                    HasError = true;
                }
                else
                {
                    ErrorMessage = string.Empty;
                    HasError = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to drop course: {ex.Message}";
                HasError = true;
            }
        }
    }
}