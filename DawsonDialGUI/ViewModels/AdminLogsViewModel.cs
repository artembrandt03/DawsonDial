using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDialGUI.Views;
using System.Linq;

namespace DawsonDialGUI.ViewModels;

public partial class AdminLogsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<AdminLogDisplay> logs = new();

    public AdminLogsViewModel(Admin admin, DawsonDialService service, Window mainWindow)
    {
        Initialize(admin, service, mainWindow);
        LoadLogs();
    }

    public class AdminLogDisplay
    {
        public string Timestamp { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
    }

    private async void LoadLogs()
    {
        var entries = await Service.GetAllLogsAsync();
        Logs = new ObservableCollection<AdminLogDisplay>();

        foreach (var entry in entries.OrderByDescending(log => log.Timestamp))
        {
            Logs.Add(new AdminLogDisplay
            {
                Timestamp = entry.Timestamp.ToLocalTime().ToString("yyyy-MM-dd h:mm:ss tt"),
                Username = entry.Admin.Username,
                Action = entry.Action
            });
        }
    }

    protected override void GoBackToMenu()
    {
        SetCurrentView(new AdminMenuView
        {
            DataContext = new AdminMenuViewModel((Admin)LoggedInUser, Service, MainWindow)
        });
    }
}