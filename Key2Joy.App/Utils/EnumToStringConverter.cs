using System;
using Microsoft.UI.Xaml.Data;

namespace Key2Joy.App.Utils;

public partial class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => Enum.GetName(value.GetType(), value) ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
