using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TotallyNormalCalculator.MVVM.ViewModels;

namespace TotallyNormalCalculator.Converter;

public class ViewModelToGridLengthConverter : IValueConverter
{
    // Hide SecretViewViewModel when Calculator is selected
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is CalculatorViewModel ? new GridLength(0) : GridLength.Auto;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
