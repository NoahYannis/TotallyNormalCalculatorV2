using System;
using System.Globalization;
using System.Windows.Data;

namespace TotallyNormalCalculator.Converter;

public class NullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not null ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
