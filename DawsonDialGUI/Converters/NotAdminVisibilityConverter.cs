using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DawsonDialGUI.Converters
{
    public class NotAdminVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string role && role.ToLower() != "admin";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}