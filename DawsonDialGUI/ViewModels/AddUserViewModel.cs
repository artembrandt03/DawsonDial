using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class AddUserViewModel : ViewModelBase
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private int age;
    [ObservableProperty] private string userType = "Student";

    // Student fields
    [ObservableProperty] private string program = string.Empty;
    [ObservableProperty] private string studentId = string.Empty;
    [ObservableProperty] private string year = string.Empty;

    // Teacher field
    [ObservableProperty] private string department = string.Empty;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public List<string> UserTypes { get; } = new() { "Student", "Teacher", "Admin" };

    public bool IsStudent => UserType == "Student";
    public bool IsTeacher => UserType == "Teacher";

    public IRelayCommand CreateCommand { get; }

    public AddUserViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        CreateCommand = new AsyncRelayCommand(CreateUser);
    }

    private async Task CreateUser()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
            switch (UserType)
            {
                case "Student":
                    int parsedStudentId = int.TryParse(StudentId, out var sid) ? sid : throw new Exception("Invalid Student ID");
                    int parsedYear = int.TryParse(Year, out var yr) ? yr : throw new Exception("Invalid Year");
                    await Service.CreateStudent(username, password, firstName, lastName, age, parsedStudentId, program, parsedYear);
                    break;

                case "Teacher":
                    await Service.CreateTeacher(username, password, firstName, lastName, age, department);
                    break;

                case "Admin":
                    await Service.CreateAdmin(username, password, firstName, lastName, age);
                    break;

                default:
                    throw new InvalidOperationException("Unknown user type selected.");
            }

            SuccessMessage = "User added successfully.";
            ((Admin)LoggedInUser).IncrementUserCreationCount();
            await Service.LogAction((Admin)LoggedInUser, $"Created {UserType.ToLower()} {Username}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to add user: {ex.Message}";
        }
    }

    partial void OnUserTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsStudent));
        OnPropertyChanged(nameof(IsTeacher));
    }
    protected override void GoBackToMenu()
    {
        SetCurrentView(new UserMenuView
        {
            DataContext = new UserMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}