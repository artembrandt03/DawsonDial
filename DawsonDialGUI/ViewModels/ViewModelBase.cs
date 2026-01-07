using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DawsonDialGUI.Views.StudentViews;

namespace DawsonDialGUI.ViewModels;

public partial class ViewModelBase : ObservableObject
{

    // Core Services
    protected Person LoggedInUser { get; private set; } = null!;
    protected DawsonDialService Service { get; private set; } = null!;
    protected Window MainWindow { get; private set; } = null!;

    // Shared Commands
    public ICommand GoBackToMenuCommand { get; private set; } = null!;
    public ICommand ViewProfileCommand { get; private set; } = null!;
    public ICommand ChangePasswordCommand { get; private set; } = null!;
    public ICommand LogoutCommand { get; private set; } = null!;


    /// <summary>
    /// Initializes the shared view model state and commands.
    /// </summary>
    /// <param name="loggedInUser">The currently logged in user.</param>
    /// <param name="service">The DawsonDialService manager.</param>
    /// <param name="mainWindow">The main avalonia window.</param>
    public void Initialize(Person loggedInUser, DawsonDialService service, Window mainWindow)
    {
        LoggedInUser = loggedInUser ?? throw new ArgumentNullException(nameof(loggedInUser), "Logged in user cannot be null");
        MainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow), "Main window cannot be null");
        Service = service ?? throw new ArgumentNullException(nameof(service), "Service cannot be null");

        GoBackToMenuCommand = new RelayCommand(GoBackToMenu);
        ViewProfileCommand = new RelayCommand<Person>(ViewProfile);
        ChangePasswordCommand = new RelayCommand(NavigateToChangePassword);
        LogoutCommand = new RelayCommand(Logout);
    }

    /// <summary>
    /// Navigates back to the menu based on the type of logged-in user.
    /// </summary>
    protected virtual void GoBackToMenu()
    {
        if (LoggedInUser is Student student)
        {
            SetCurrentView(new StudentMenuView
            {
                DataContext = new StudentMenuViewModel(student, Service, MainWindow)
            });
        }
        else if (LoggedInUser is Teacher teacher)
        {
            SetCurrentView(new Views.TeacherViews.TeacherMenuView
            {
                DataContext = new ViewModels.TeacherViewModels.TeacherMenuViewModel(teacher, Service, MainWindow)
            });
        }
        else if (LoggedInUser is Admin admin)
        {
            SetCurrentView(new AdminMenuView
            {
                DataContext = new AdminMenuViewModel(admin, Service, MainWindow)
            });
        }
        else
        {
            throw new InvalidOperationException("Unrecognized user type.");
        }
    }

    /// <summary>
    /// Sets the current view in the main window.
    /// </summary>
    /// <param name="view">The view to set.</param>
    public void SetCurrentView(UserControl view)
    {
        if (MainWindow?.DataContext is MainWindowViewModel mainViewModel)
        {
            mainViewModel.CurrentView = view;
        }
    }

    /// <summary>
    /// Displays the profile view of the specified user.
    /// </summary>
    /// <param name="user">The user to view the profile of.</param>
    public void ViewProfile(Person? user)
    {
        var profileView = new Views.ViewProfileView
        {
            DataContext = new ViewProfileViewModel(user, Service, MainWindow)
        };

        SetCurrentView(profileView);
    }

    /// <summary>
    /// Navigates to the change password view.
    /// </summary>
    [RelayCommand]
    private void NavigateToChangePassword()
    {
        SetCurrentView(new ChangePasswordView
        {
            DataContext = new ChangePasswordViewModel(LoggedInUser, Service, MainWindow)
        });
    }


    /// <summary>
    /// Logs out the student and shows the login view.
    /// </summary>
    protected async void Logout()
    {
        if (MainWindow != null)
        {
            if (MainWindow.DataContext is MainWindowViewModel mainViewModel)
            {
                if (LoggedInUser is Admin admin)
                {
                    await Service.LogAction(admin, "Logged out");
                }
                LoggedInUser = null!;
                mainViewModel.ShowLoginView();
            }
        }
    }
}
