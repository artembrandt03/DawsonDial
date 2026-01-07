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

public partial class UpdateProfileViewModel : ViewModelBase
{
    // Bound properties
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public UpdateProfileViewModel(Person user, DawsonDialService service, Window mainWindow)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        Initialize(user, service, mainWindow);
    }

    [RelayCommand]
    private async Task UpdateProfile()
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "First name and last name cannot be empty.";
            return;
        }

        // Update user profile
        LoggedInUser.FirstName = FirstName;
        LoggedInUser.LastName = LastName;
        LoggedInUser.Description = Description;

        try{
            await Service.UpdateUserProfile(LoggedInUser);
            ViewProfileWithSuccess("Profile updated successfully.");
        }
        catch (Exception ex)
        {
            ViewProfileWithError($"Failed to update profile: {ex.Message}");
        }
    }

    /// <summary>
    /// Navigates to the profile view of the logged-in user.
    /// </summary>
    [RelayCommand]
    private void NavigateToProfile()
    {
        ViewProfile(LoggedInUser);
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
}
