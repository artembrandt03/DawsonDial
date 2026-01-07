using Avalonia.Controls;
using Avalonia.Interactivity;
using DawsonDial.Models.Rooms;
using DawsonDialGUI.ViewModels;

namespace DawsonDialGUI.Views
{
    public partial class ViewAllRoomsView : UserControl
    {
        public ViewAllRoomsView()
        {
            InitializeComponent();
        }

    private void OnModifyRoomClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ViewAllRoomsViewModel.RoomDisplay roomDisplay)
        {
            if (DataContext is ViewAllRoomsViewModel vm)
            {
                vm.OnModifyRoomClick(roomDisplay);
            }
        }
    }
    }
}