using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Key2Joy.App.Utils;

/// <summary>
/// Provides a value converter that returns a visible state when the input is zero, and a collapsed state
/// otherwise.
/// </summary>
public class VisibleWhenZeroConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not int number)
        {
            return Visibility.Collapsed;
        }

        return number == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

