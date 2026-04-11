using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Windows;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Windows;

[MappingControl(
    ForTypes = new[] { typeof(WindowMinimizeAction), typeof(WindowFocusAction) },
    ImageResourceName = "ms-appx:///Assets/Icons/application_xp_terminal.png"
)]
public sealed partial class WindowActionControl : UserControl
{
    public WindowActionControl()
    {
        this.InitializeComponent();
    }
}
