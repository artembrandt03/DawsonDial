using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDial.Models.Rooms;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

public partial class RoomDetailsViewModel : ViewModelBase
{
    // Bound properties
    [ObservableProperty] private Room room = null!;
    [ObservableProperty] private string roomType = string.Empty;
    [ObservableProperty] private string numberOfSeats = string.Empty;
    [ObservableProperty] private string hasProjector = string.Empty;
    [ObservableProperty] private string hasComputers = string.Empty;

    public IRelayCommand NavigateBackCommand { get; }

    // Computed properties
    public string Title => $"Room {Room.RoomNumber}'s Details";


    /// <summary>
    /// Initializes a new instance of the <see cref="RoomDetailsViewModel"/> class.
    /// </summary>
    /// <param name="room">The selected room to view</param>
    /// <param name="user">The user viewing the room</param>
    /// <param name="service">The Dawson manager service</param>
    /// <param name="mainWindow">The main avalonia window</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null</exception>
    public RoomDetailsViewModel(Room room, Person? user, DawsonDialService service, Window mainWindow)
    {
        if (room == null) throw new ArgumentNullException(nameof(room));
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (service == null) throw new ArgumentNullException(nameof(service));
        if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));
        Initialize(user, service, mainWindow);

        // Initialize fields
        Room = room;
        RoomType = room switch
        {
            ClassRoom => "Room Type: Class Room",
            ConferenceRoom => "Room Type: Conference Room",
            _ => "Room Type: Unknown Room Type"
        };
        NumberOfSeats = $"Number of Seats: {room.NumberOfSeats}";

        if (room is ClassRoom classroom)
        {
            HasComputers = classroom.HasComputers ? "Has Computers: Yes" : "Has Computers: No";
            HasProjector = classroom.HasProjector ? "Has Projector: Yes" : "Has Projector: No";
        }
        if (room is ConferenceRoom conferenceRoom)
        {
            HasComputers = "Has Computers: No";
            HasProjector = conferenceRoom.HasProjector ? "Has Projector: Yes" : "Has Projector: No";
        }
        NavigateBackCommand = new RelayCommand(() => NavigateBack());
    }

    /// <summary>
    /// Navigates back to the previous view.
    /// </summary>
    public void NavigateBack()
    {
        var previousView = new Views.TeacherViews.ViewRoomAvailabilityView
        {
            DataContext = new ViewRoomAvailabilityViewModel((Teacher)LoggedInUser, Service, MainWindow)
        };

        SetCurrentView(previousView);
    }
}
