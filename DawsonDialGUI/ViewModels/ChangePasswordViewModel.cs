using System;
using System.Windows.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;

namespace DawsonDialGUI.ViewModels;

public partial class ChangePasswordViewModel : ViewModelBase
{
    // Bound properties
    [ObservableProperty] private string currentPassword = string.Empty;
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    /// <summary>
    /// Initializes the ChangePasswordViewModel with the provided user, service, and main window.
    /// </summary>
    /// <param name="user">The logged in user.</param>
    /// <param name="service">The DawsonDialService manager.</param>
    /// <param name="mainWindow">The main avalonia window.</param>
    public ChangePasswordViewModel(Person user, DawsonDialService service, Window mainWindow)
    {
        Initialize(user, service, mainWindow);
    }

    /// <summary>
    /// Submits the password change request.
    /// </summary>
    /// <returns>Async task</returns>
    [RelayCommand]
    private async Task SubmitPasswordChange()
    {
        ErrorMessage = string.Empty;

        // Validate input
        if (string.IsNullOrWhiteSpace(CurrentPassword) ||
            string.IsNullOrWhiteSpace(NewPassword) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "New passwords do not match.";
            return;
        }

        if (NewPassword.Length < 8)
        {
            ErrorMessage = "New password must be at least 8 characters long.";
            return;
        }

        try
        {
            await Service.ChangeUserPassword(LoggedInUser.Username, CurrentPassword, NewPassword);
            ViewProfileWithSuccess("Password changed successfully.");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            ViewProfileWithError("Failed to change password. Please try again.");
            return;
        }
    }

    /// <summary>
    /// Displays the profile view with a success message.
    /// </summary>
    /// <param name="message">A success message</param>
    private void ViewProfileWithSuccess(string message)
    {
        var profileView = new Views.ViewProfileView
        {
            DataContext = new ViewProfileViewModel(LoggedInUser, Service, MainWindow, message)
        };

        SetCurrentView(profileView);
    }

    /// <summary>
    /// Displays the profile view with an error message.
    /// </summary>
    /// <param name="message">An error message</param>
    private void ViewProfileWithError(string message)
    {
        var profileView = new Views.ViewProfileView
        {
            DataContext = new ViewProfileViewModel(LoggedInUser, Service, MainWindow, message)
        };
        SetCurrentView(profileView);
    }

    /// <summary>
    /// Navigates to the profile view of the logged-in user.
    /// </summary>
    [RelayCommand]
    private void NavigateToProfile()
    {
        ViewProfile(LoggedInUser);
    }
}
