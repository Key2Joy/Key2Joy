using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Key2Joy.LowLevelInput;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Key2Joy.App.UserControls;

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

    /// <summary>Identifies the <see cref="DeviceIndex"/> dependency property.</summary>
    public static readonly DependencyProperty DeviceIndexProperty =
        DependencyProperty.Register(
            nameof(DeviceIndex),
            typeof(int),
            typeof(DeviceControl),
            new PropertyMetadata(0, OnDeviceIndexChanged));

    /// <summary>Gets or sets the one-based index shown in the header.</summary>
    public int DeviceIndex
    {
        get => (int)GetValue(DeviceIndexProperty);
        set => SetValue(DeviceIndexProperty, value);
    }

    private static void OnDeviceIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DeviceControl)d;
        control.IndexTextBlock.Text = $"#{e.NewValue}";
    }

    /// <summary>Identifies the <see cref="DeviceName"/> dependency property.</summary>
    public static readonly DependencyProperty DeviceNameProperty =
        DependencyProperty.Register(
            nameof(DeviceName),
            typeof(string),
            typeof(DeviceControl),
            new PropertyMetadata(string.Empty, OnDeviceNameChanged));

    /// <summary>Gets or sets the device name shown below the header.</summary>
    public string DeviceName
    {
        get => (string)GetValue(DeviceNameProperty);
        set => SetValue(DeviceNameProperty, value);
    }

    private static void OnDeviceNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DeviceControl)d;
        control.DeviceTextBlock.Text = e.NewValue as string ?? string.Empty;
    }
}
