using System.Collections.Generic;
using DependencyPropertyGenerator;
using Key2Joy.App.UserControls.Triggers;
using Key2Joy.LowLevelInput;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls;

[DependencyProperty<int>("DeviceIndex")] //OnDeviceIndexChanged
[DependencyProperty<string>("DeviceName")] //OnDeviceNameChanged
public sealed partial class DeviceControl : UserControl
{
    public DeviceControl()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Initialises the control and populates it with device information.
    /// </summary>
    /// <param name="device">The game-pad whose details should be displayed.</param>
    public DeviceControl(IGamePadInfo device)
        : this()
    {
        this.IndexTextBlock.Text = $"#{device.Index}";
        this.DeviceTextBlock.Text = device.Name;
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
