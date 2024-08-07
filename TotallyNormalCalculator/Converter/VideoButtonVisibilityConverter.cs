﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TotallyNormalCalculator.MVVM.Model.Blobs;

namespace TotallyNormalCalculator.Converter;
internal class VideoButtonVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Any(o => o is null))
            return Visibility.Visible;

        var mediaElementUrl = ((values[0] as MediaElement).DataContext as VideoBlob)?.VideoUrl;
        VideoBlob selectedVideo = values[1] as VideoBlob;

        if (mediaElementUrl == selectedVideo?.VideoUrl)
            return Visibility.Hidden;

        return Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
