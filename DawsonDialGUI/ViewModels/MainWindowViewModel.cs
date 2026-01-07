using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Services.ManagerService;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDialGUI.Views;
using Avalonia.Controls;
using DawsonDialGUI.Views.StudentViews;

namespace DawsonDialGUI.ViewModels
{
    /// <summary>
    /// Represents the view model for the main window.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        // Dependencies
        private readonly DawsonDialService _service;
        private Window _mainWindow;

        // Current View in the main content area
        [ObservableProperty] private UserControl currentView = null!;

        // Shared login/register form fields
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string confirmPassword = string.Empty;
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string age = string.Empty;
        [ObservableProperty] private string studentId = string.Empty;
        [ObservableProperty] private string program = string.Empty;
        [ObservableProperty] private string year = string.Empty;
        [ObservableProperty] private string errorMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="service">The service used for authentication.</param>
        /// <param name="mainWindow">The main window.</param>
        public MainWindowViewModel(DawsonDialService service, Window mainWindow)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null");
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow), "Main window cannot be null");
            ShowLoginView();
        }

        /// <summary>
        /// Shows the login view.
        /// </summary>
        [RelayCommand]
        public void ShowLoginView()
        {
            ClearAllfields();
            CurrentView = new LoginView { DataContext = this };
        }

        /// <summary>
        /// Shows the register view.
        /// </summary>
        [RelayCommand]
        private void ShowRegisterView()
        {
            ClearAllfields();
            CurrentView = new RegisterView { DataContext = this };
        }

        /// <summary>
        /// Attempts to log in the user with the provided credentials.
        /// </summary>
        [RelayCommand]
        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;
            await _service.MarkPastEventsAsCompleted();

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password cannot be empty.";
                return;
            }

            try
            {
                if (!await _service.AuthenticateUser(Username, Password))
                {
                    ErrorMessage = "Invalid username or password.";
                    return;
                }

                var user = await _service.GetUserByUsername(Username);
                if (user == null)
                {
                    ErrorMessage = "User not found.";
                    return;
                }

                await NavigateToUserDashboard(user);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [RelayCommand]
        private async Task Register()
        {
            ErrorMessage = string.Empty;

            // Validate input fields
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Age) ||
                string.IsNullOrWhiteSpace(StudentId) ||
                string.IsNullOrWhiteSpace(Program) ||
                string.IsNullOrWhiteSpace(Year))
            {
                ErrorMessage = "All fields are required.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            // Parse and validate integer values
            if (!int.TryParse(Age, out int parsedAge) || parsedAge <= 9)
            {
                ErrorMessage = "Invalid age.";
                return;
            }

            if (!int.TryParse(StudentId, out int parsedStudentId))
            {
                ErrorMessage = "Invalid Student ID.";
                return;
            }

            if (!int.TryParse(Year, out int parsedYear) || parsedYear < 1 || parsedYear > 3)
            {
                ErrorMessage = "Year must be between 1 and 3.";
                return;
            }

            // Create a new student object
            var student = new Student(
                Username,
                Password,
                FirstName,
                LastName,
                parsedAge,
                false,
                "",
                parsedStudentId,
                Program,
                parsedYear,
                new HashSet<Section>());

            try
            {
                // Register the student
                await _service.RegisterUser(student);
                ErrorMessage = "Registration successful!";
                ShowLoginView();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
                return;
            }
        }

        /// <summary>
        /// Navigates to the user dashboard based on the user type.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task NavigateToUserDashboard(Person user)
        {
            switch (user)
            {
                case Student student:
                    CurrentView = new StudentMenuView
                    {
                        DataContext = new StudentMenuViewModel(student, _service, _mainWindow)
                    };
                    break;

                case Teacher teacher:
                    CurrentView = new Views.TeacherViews.TeacherMenuView
                    {
                        DataContext = new ViewModels.TeacherViewModels.TeacherMenuViewModel(teacher, _service, _mainWindow)
                    };
                    break;

                case Admin admin:
                    await _service.LogAction(admin, "Logged in");
                    CurrentView = new AdminMenuView
                    {
                        DataContext = new AdminMenuViewModel(admin, _service, _mainWindow)
                    };
                    break;

                default:
                    ErrorMessage = "Unsupported user type.";
                    break;
            }
        }


        /// <summary>
        /// Clears all input fields in the registration/login form.
        /// </summary>
        private void ClearAllfields()
        {
            ErrorMessage = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Age = string.Empty;
            StudentId = string.Empty;
            Program = string.Empty;
            Year = string.Empty;
        }
    }
}
