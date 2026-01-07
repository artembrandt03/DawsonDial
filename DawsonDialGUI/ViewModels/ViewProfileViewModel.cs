using System;
using System.Windows.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using System.Runtime.CompilerServices;

namespace DawsonDialGUI.ViewModels;

public partial class ViewProfileViewModel : ViewModelBase
{
    // Bound properties
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private int studentId;
    [ObservableProperty] private string program = string.Empty;
    [ObservableProperty] private int year;
    [ObservableProperty] private string department = string.Empty;
    [ObservableProperty] private int createdUsers;
    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
    public string Title => $"{FullName}'s Profile";
    public bool IsStudent => LoggedInUser is Student;
    public bool IsTeacher => LoggedInUser is Teacher;
    public bool IsAdmin => LoggedInUser is Admin;
    public bool ShowDescription => !string.IsNullOrEmpty(Description);

    // Menu Commands
    public IRelayCommand NavigateToUpdateProfileCommand { get; }

    /// <summary>
    /// Initializes the ViewProfileViewModel with the provided user, service, and main window.
    /// </summary>
    /// <param name="user">The logged in user.</param>
    /// <param name="service">The DawsonDialService maanager.</param>
    /// <param name="mainWindow">The main avalonia window.</param>
    /// <param name="successMessage">A success message.</param>
    /// <param name="errorMessage">An error message.</param>
    /// <exception cref="ArgumentNullException">If the user is null.</exception>
    public ViewProfileViewModel(Person? user, DawsonDialService service, Window mainWindow, string successMessage = "", string errorMessage = "")
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        Initialize(user, service, mainWindow);
        SuccessMessage = successMessage;
        ErrorMessage = errorMessage;

        // Initialize shared fields
        Username = user.Username;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Description = user.Description!;

        // Student only fields
        if (user is Student student)
        {
            StudentId = student.StudentID;
            Program = student.Program;
            Year = student.Year;
        }

        // Teacher only fields
        if (user is Teacher teacher)
        {
            Department = teacher.Department;
        }

        // Admin only fields
        if (user is Admin admin)
        {
            CreatedUsers = admin.CreatedUsersCount;
        }

        NavigateToUpdateProfileCommand = new RelayCommand(() => NavigateToUpdateProfile());
    }

    /// <summary>
    /// Displays the update profile view for the specified user.
    /// </summary>
    /// <param name="user">The user to update the proile of</param>
    public void NavigateToUpdateProfile()
    {
        var updateProfileView = new Views.UpdateProfileView
        {
            DataContext = new UpdateProfileViewModel(LoggedInUser, Service, MainWindow)
        };

        SetCurrentView(updateProfileView);
    }
}
