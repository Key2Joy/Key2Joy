using System;
using Microsoft.UI.Xaml.Data;

namespace Key2Joy.App.Utils;

public partial class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => !string.IsNullOrEmpty(value as string);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
