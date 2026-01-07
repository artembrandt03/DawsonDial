using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class ViewAllRoomsViewModel : ViewModelBase
{
    public class RoomDisplay
    {
        public string RoomNumber { get; set; } = "";
        public string Type { get; set; } = "";
        public int Seats { get; set; }
        public Room OriginalRoom { get; set; } = null!;
    }

    [ObservableProperty]
    private ObservableCollection<RoomDisplay> rooms = new();

    public ViewAllRoomsViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        LoadRooms();
    }

    private async void LoadRooms()
    {
        var all = await Service.GetAllRoomsAsync();
        Rooms = new ObservableCollection<RoomDisplay>(
            all.Select(r => new RoomDisplay
            {
                RoomNumber = r.RoomNumber,
                Seats = r.NumberOfSeats,
                Type = r.GetType().Name,
                OriginalRoom = r
            })
        );
    }

    public void OnModifyRoomClick(RoomDisplay roomDisplay)
    {
        SetCurrentView(new ModifyRoomView
        {
            DataContext = new ModifyRoomViewModel(roomDisplay.OriginalRoom, (Admin)LoggedInUser, Service, MainWindow)
        });
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new RoomMenuView
        {
            DataContext = new RoomMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}