using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Models.Rooms;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class AddRoomViewModel : ViewModelBase
{
    [ObservableProperty] private string roomNumber = string.Empty;
    [ObservableProperty] private string numberOfSeats = string.Empty;
    [ObservableProperty] private string selectedRoomType = "Office";

    // Room type options
    public List<string> RoomTypes { get; } = new() { "Office", "ClassRoom", "ConferenceRoom" };

    // Conditional fields
    [ObservableProperty] private bool isShared;
    [ObservableProperty] private bool hasProjector;
    [ObservableProperty] private bool hasComputers;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public bool IsOffice => SelectedRoomType == "Office";
    public bool IsClassRoom => SelectedRoomType == "ClassRoom";
    public bool IsConferenceRoom => SelectedRoomType == "ConferenceRoom";

    public IRelayCommand AddRoomCommand { get; }

    public AddRoomViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        AddRoomCommand = new AsyncRelayCommand(AddRoom);
    }

    partial void OnSelectedRoomTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsOffice));
        OnPropertyChanged(nameof(IsClassRoom));
        OnPropertyChanged(nameof(IsConferenceRoom));
    }

    private async Task AddRoom()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
            if (!int.TryParse(NumberOfSeats, out var seats))
                throw new Exception("Invalid number of seats.");

            Room newRoom = SelectedRoomType switch
            {
                "Office" => new Office(RoomNumber, seats, IsShared),
                "ClassRoom" => new ClassRoom(RoomNumber, seats, HasProjector, HasComputers),
                "ConferenceRoom" => new ConferenceRoom(RoomNumber, seats, HasProjector),
                _ => throw new Exception("Unknown room type.")
            };

            await Service.AddRoomAsync(newRoom);
            SuccessMessage = "Room added successfully.";
            await Service.LogAction((Admin)LoggedInUser, $"Created {SelectedRoomType} {RoomNumber}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to add room: {ex.Message}";
        }
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new RoomMenuView
        {
            DataContext = new RoomMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}