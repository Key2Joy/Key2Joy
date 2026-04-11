using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

[MappingControl(
    ForType = typeof(WaitAction),
    ImageResourceName = "ms-appx:///Assets/Icons/clock.png"
)]
public sealed partial class WaitActionControl : UserControl
{
    public WaitActionControl()
    {
        this.InitializeComponent();
    }
}
