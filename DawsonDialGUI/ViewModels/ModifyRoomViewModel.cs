using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class ModifyRoomViewModel : ViewModelBase
{
    private readonly Room _originalRoom;

    [ObservableProperty] private string roomNumber;
    [ObservableProperty] private string numberOfSeats;
    [ObservableProperty] private bool hasProjector;
    [ObservableProperty] private bool hasComputers;
    [ObservableProperty] private bool isShared;

    [ObservableProperty] private string successMessage = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    public IRelayCommand SaveCommand { get; }

    public bool IsClassroom => _originalRoom is ClassRoom;
    public bool IsConferenceRoom => _originalRoom is ConferenceRoom;
    public bool IsOffice => _originalRoom is Office;

    public ModifyRoomViewModel(Room room, Admin admin, DawsonDialService service, Window mainWindow)
    {
        _originalRoom = room;
        Initialize(admin, service, mainWindow);
        SaveCommand = new AsyncRelayCommand(Save);

        //Prefill UI fields
        RoomNumber = room.RoomNumber;
        NumberOfSeats = room.NumberOfSeats.ToString();

        if (room is ClassRoom classroom)
        {
            HasProjector = classroom.HasProjector;
            HasComputers = classroom.HasComputers;
        }
        else if (room is ConferenceRoom conference)
        {
            HasProjector = conference.HasProjector;
        }
        else if (room is Office office)
        {
            IsShared = office.Shared;
        }
    }

    private async Task Save()
    {
        ErrorMessage = SuccessMessage = string.Empty;

        try
        {
            if (!int.TryParse(NumberOfSeats, out var seats))
                throw new Exception("Invalid number of seats.");

            _originalRoom.RoomNumber = RoomNumber;
            _originalRoom.NumberOfSeats = seats;

            switch (_originalRoom)
            {
                case ClassRoom classroom:
                    classroom.HasProjector = HasProjector;
                    classroom.HasComputers = HasComputers;
                    break;
                case ConferenceRoom conference:
                    conference.HasProjector = HasProjector;
                    break;
                case Office office:
                    office.Shared = IsShared;
                    break;
            }

            await Service.UpdateRoomAsync(_originalRoom);
            await Service.LogAction((Admin)LoggedInUser, $"Modified room {_originalRoom.RoomNumber}");

            SuccessMessage = "Room updated successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update room: {ex.Message}";
        }
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new ViewAllRoomsView
        {
            DataContext = new ViewAllRoomsViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}
