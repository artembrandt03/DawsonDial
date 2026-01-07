using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;


public partial class ViewAllUsersViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Person> users = new();

    [RelayCommand]
    private void ModifyUser(Person person)
    {
        OpenModifyUserView(person);
    }

    public ViewAllUsersViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        LoadUsers();
    }

    private async void LoadUsers()
    {
        var result = await Service.GetAllUsersAsync();
        Users = new ObservableCollection<Person>(result);
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new UserMenuView
        {
            DataContext = new UserMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    public void OpenModifyUserView(Person user)
    {
        SetCurrentView(new ModifyUserView
        {
            DataContext = new ModifyUserViewModel(user, LoggedInUser, Service, MainWindow)
        });
    }
}