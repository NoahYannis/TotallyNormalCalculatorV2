using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TotallyNormalCalculator.Converter;

public class NullToVisibilityConverter : IValueConverter
{
    public bool NullEqualsVisible { get; set; }


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = value is not null;

        if (NullEqualsVisible)
        {
            isVisible = !isVisible;
        }

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
