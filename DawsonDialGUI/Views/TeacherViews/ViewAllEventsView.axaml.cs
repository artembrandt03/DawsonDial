using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DawsonDialGUI.Views.TeacherViews
{
    public partial class ViewAllEventsView : UserControl
    {
        public ViewAllEventsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}