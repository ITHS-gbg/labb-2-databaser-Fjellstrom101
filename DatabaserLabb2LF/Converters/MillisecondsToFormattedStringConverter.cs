using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DatabaserLabb2LF.Converters;

public class MillisecondsToFormattedStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value is not int) return DependencyProperty.UnsetValue;

        int milliseconds = (int)value;

        if ((milliseconds/3600000) > 0)
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