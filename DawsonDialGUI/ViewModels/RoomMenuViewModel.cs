using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public class RoomMenuViewModel : ViewModelBase
{
    public ICommand ViewAllRoomsCommand { get; }
    public ICommand AddRoomCommand { get; }
    public ICommand GoBackToMenuCommand { get; }

    public RoomMenuViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);

        ViewAllRoomsCommand = new RelayCommand(OpenViewAllRooms);
        AddRoomCommand = new RelayCommand(OpenAddRoom);
        GoBackToMenuCommand = new RelayCommand(GoBack);
    }

    private void OpenViewAllRooms()
    {
        SetCurrentView(new ViewAllRoomsView
        {
            DataContext = new ViewAllRoomsViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void OpenAddRoom()
    {
        SetCurrentView(new AddRoomView
        {
            DataContext = new AddRoomViewModel((Admin)LoggedInUser, Service, MainWindow)
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