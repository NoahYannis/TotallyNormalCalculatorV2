﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TotallyNormalCalculator.Converter;

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str && string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
