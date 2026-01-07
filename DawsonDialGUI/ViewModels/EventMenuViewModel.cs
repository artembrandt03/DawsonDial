using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public class EventMenuViewModel : ViewModelBase
{
    public ICommand ViewAllEventsCommand { get; }
    public ICommand AddEventCommand { get; }
    public ICommand GoBackToMenuCommand { get; }

    public EventMenuViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);

        // ViewAllEventsCommand = new RelayCommand(OpenViewAllEvents);
        AddEventCommand = new RelayCommand(OpenAddEvent);
        GoBackToMenuCommand = new RelayCommand(GoBack);
    }

    // private void OpenViewAllEvents()
    // {
    //     SetCurrentView(new ViewAllEventsView
    //     {
    //         DataContext = new ViewAllEventsViewModel((Admin)LoggedInUser, Service, MainWindow)
    //     });
    // }

    private void OpenAddEvent()
    {
        SetCurrentView(new AddEventView
        {
            DataContext = new AddEventViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void GoBack()
    {
        SetCurrentView(new AdminMenuView
        {
            DataContext = new AdminMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}