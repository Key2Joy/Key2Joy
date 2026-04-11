using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Input;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Input;

[MappingControl(
    ForType = typeof(GamePadButtonAction),
    ImageResourceName = "ms-appx:///Assets/Icons/joystick.png"
)]
public sealed partial class GamePadActionControl : UserControl
{
    public GamePadActionControl()
    {
        this.InitializeComponent();
    }
}
