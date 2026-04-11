using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Input;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Input;

[MappingControl(
    ForType = typeof(KeyboardAction),
    ImageResourceName = "ms-appx:///Assets/Icons/keyboard.png"
)]
public sealed partial class KeyboardActionControl : UserControl
{
    public KeyboardActionControl()
    {
        this.InitializeComponent();
    }
}
