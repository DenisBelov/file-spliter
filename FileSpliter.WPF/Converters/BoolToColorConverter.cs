using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FileSpliter.WPF.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? new SolidColorBrush(Color.FromRgb(0, 184, 148)) : new SolidColorBrush(Color.FromRgb(214, 48, 49));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
