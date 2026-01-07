using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class ModifyUserViewModel : ViewModelBase
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private int age;

    [ObservableProperty] private string studentId = string.Empty;
    [ObservableProperty] private string program = string.Empty;
    [ObservableProperty] private string year = string.Empty;
    [ObservableProperty] private string department = string.Empty;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public bool IsStudent => _user is Student;
    public bool IsTeacher => _user is Teacher;

    private readonly Person _user;

    public IRelayCommand SaveCommand { get; }

    public ModifyUserViewModel(Person userToEdit, Person adminUser, DawsonDialService service, Window mainWindow)
    {
        _user = userToEdit;
        Initialize(adminUser, service, mainWindow);

        // Pre-fill values
        Username = userToEdit.Username;
        FirstName = userToEdit.FirstName;
        LastName = userToEdit.LastName;
        Age = userToEdit.Age;

        if (userToEdit is Student s)
        {
            StudentId = s.StudentID.ToString();
            Program = s.Program;
            Year = s.Year.ToString();
        }
        else if (userToEdit is Teacher t)
        {
            Department = t.Department;
        }

        SaveCommand = new RelayCommand(Save);
    }

    private async void Save()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
            _user.Username = Username;
            _user.FirstName = FirstName;
            _user.LastName = LastName;
            _user.Age = Age;

            if (_user is Student s)
            {
                s.Program = Program;
                s.Year = int.TryParse(Year, out var yr) ? yr : s.Year;
                s.StudentID = int.TryParse(StudentId, out var id) ? id : s.StudentID;
            }
            else if (_user is Teacher t)
            {
                t.Department = Department;
            }

            await Service.UpdateUserProfile(_user);
            await Service.LogAction((Admin)LoggedInUser, $"Modified user {_user.Username}");

            SuccessMessage = "User updated successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update: {ex.Message}";
        }
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new ViewAllUsersView
        {
            DataContext = new ViewAllUsersViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}
