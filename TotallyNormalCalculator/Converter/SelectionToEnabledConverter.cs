using System;
using System.Globalization;
using System.Windows.Data;
using TotallyNormalCalculator.MVVM.Model;

namespace TotallyNormalCalculator.Converter;

public class SelectionToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is IModel;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
