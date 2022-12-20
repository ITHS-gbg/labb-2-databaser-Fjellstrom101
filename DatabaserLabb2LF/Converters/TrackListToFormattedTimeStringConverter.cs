using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DatabaserLabb2LF.DataAccess.DbModels;

namespace DatabaserLabb2LF.Converters;

public class TrackListToFormattedTimeStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value is not IEnumerable<Track>) return DependencyProperty.UnsetValue;

        var list = (IEnumerable<Track>)value;

        long milliseconds = list.Aggregate<Track, long>(0, (current, track) => current + track.Milliseconds);

        if ((milliseconds / 3600000) > 0)
        {
            return (milliseconds / 3600000).ToString() +
                   ':' + ((milliseconds % 3600000) / 60000).ToString("D2") +
                   ":" + ((milliseconds % 60000) / 1000).ToString("D2");
        }

        return ((milliseconds % 3600000) / 60000).ToString() +
               ':' + ((milliseconds % 60000) / 1000).ToString("D2");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}