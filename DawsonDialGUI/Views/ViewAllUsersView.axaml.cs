using Avalonia.Controls;
using Avalonia.Interactivity;
using DawsonDial.Models.People;
using DawsonDialGUI.ViewModels;

namespace DawsonDialGUI.Views;

public partial class ViewAllUsersView : UserControl
{
    public ViewAllUsersView()
    {
        InitializeComponent();
    }

    private void OnModifyUserClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Person user)
        {
            if (DataContext is ViewAllUsersViewModel vm)
            {
                vm.OpenModifyUserView(user);
            }
        }
    }
}