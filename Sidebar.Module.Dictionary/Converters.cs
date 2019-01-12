using System;
using System.Globalization;
using System.Windows.Data;

namespace Sidebar.Module.Dictionary
{
    public class LanguageImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("/Sidebar.Module.Dictionary;component/Assets/Languages/{0}.png", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ReplaceCommaWithNewlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().Replace(",", Environment.NewLine);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().Replace(Environment.NewLine, ",");
        }
    }
}
