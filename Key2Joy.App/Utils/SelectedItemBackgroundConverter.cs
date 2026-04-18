using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Key2Joy.App.Utils;

public partial class SelectedItemBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isSelected = value is bool b && b;

        var resourceKey = isSelected
            ? "AccentAcrylicBackgroundFillColorDefaultBrush"
            : "CardBackgroundFillColorDefaultBrush";

        if (Application.Current.Resources.TryGetValue(resourceKey, out var resource)
            && resource is Brush brush)
        {
            return brush;
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
