using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Models.People;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDialGUI.Views;

namespace DawsonDialGUI.ViewModels;

public partial class UserMenuViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Person> users = new();

    public IRelayCommand ViewAllUsersCommand { get; }
    public IRelayCommand AddUserCommand { get; }

    public UserMenuViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        ViewAllUsersCommand = new RelayCommand(OpenViewAllUsers);
        AddUserCommand = new RelayCommand(OpenAddUserView);
    }

    private void OpenViewAllUsers()
    {
        SetCurrentView(new ViewAllUsersView
        {
            DataContext = new ViewAllUsersViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }

    private void OpenAddUserView()
    {
        SetCurrentView(new AddUserView
        {
            DataContext = new AddUserViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}