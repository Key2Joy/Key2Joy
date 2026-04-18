using DependencyPropertyGenerator;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls;

[DependencyProperty<int>("DeviceIndex")]
[DependencyProperty<string>("DeviceName")]
public sealed partial class DeviceControl : UserControl
{
    public DeviceControl()
    {
        this.InitializeComponent();
    }

    partial void OnDeviceIndexChanged(int newValue)
    {
        this.IndexTextBlock.Text = $"#{newValue}";
    }

    partial void OnDeviceNameChanged(string newValue)
    {
        this.DeviceTextBlock.Text = newValue as string ?? string.Empty;
    }
}
