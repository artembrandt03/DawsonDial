using Avalonia.Controls;
using Avalonia.Interactivity;
using DawsonDial.Models.Events;
using DawsonDialGUI.ViewModels;

namespace DawsonDialGUI.Views
{
    public partial class ViewAllCoursesView : UserControl
    {
        public ViewAllCoursesView()
        {
            InitializeComponent();
        }

        private void OnModifyCourseClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ViewAllCoursesViewModel.CourseDisplay display)
            {
                if (DataContext is ViewAllCoursesViewModel vm)
                {
                    vm.OnModifyCourseClick(display.OriginalCourse);
                }
            }
        }
    }
}