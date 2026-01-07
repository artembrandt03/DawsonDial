using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DawsonDialGUI.Views.TeacherViews
{
    public partial class EditEventView : UserControl
    {
        public EditEventView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}