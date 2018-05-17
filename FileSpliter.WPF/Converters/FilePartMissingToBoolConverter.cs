using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using FileSpliter.Models;
using FileSpliter.WPF.ViewModels;

namespace FileSpliter.WPF.Converters
{
    public class FilePartMissingToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((IEnumerable<FilePartViewModel>) value)?.ToList().Exists(f => !f.IsAvailable) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
