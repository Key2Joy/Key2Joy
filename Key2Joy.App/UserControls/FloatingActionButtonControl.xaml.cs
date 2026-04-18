using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls;

public sealed partial class FloatingActionButtonControl : UserControl
{
    public event EventHandler? Click;

    public FloatingActionButtonControl()
        => this.InitializeComponent();

    private void FabButton_Click(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, EventArgs.Empty);
}
