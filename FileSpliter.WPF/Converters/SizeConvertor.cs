using System;
using System.Globalization;
using System.Windows.Data;
using FileSpliter.BLL.Extensions;
namespace FileSpliter.WPF.Converters
{
    public class SizeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().GetSize();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
