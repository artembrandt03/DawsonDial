using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DawsonDialGUI.Views.TeacherViews
{
    public partial class ViewMyEventsView : UserControl
    {
        public ViewMyEventsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}