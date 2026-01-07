using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DawsonDial.Models.People;
using DawsonDial.Models.Events;
using DawsonDial.Services.ManagerService;
using Avalonia.Controls;
using System.Linq;

namespace DawsonDialGUI.ViewModels.TeacherViewModels;

public partial class TeacherSectionsViewModel : ViewModelBase
{
    public ObservableCollection<Section> Sections { get; }

    public TeacherSectionsViewModel(Teacher teacher, DawsonDialService service, Window mainWindow)
    {
        Initialize(teacher, service, mainWindow);
        Sections = new ObservableCollection<Section>(
            teacher.Classes
                .OrderBy(s => s.SectionNumber)
            );
    }
}
