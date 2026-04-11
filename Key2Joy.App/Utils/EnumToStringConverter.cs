using System;
using Microsoft.UI.Xaml.Data;

namespace Key2Joy.App.Utils;

public class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return Enum.GetName(value.GetType(), value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
