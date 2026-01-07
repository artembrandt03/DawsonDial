using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DawsonDialGUI.Views.StudentViews
{
    public partial class ViewMyScheduleView : UserControl
    {
        public ViewMyScheduleView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
