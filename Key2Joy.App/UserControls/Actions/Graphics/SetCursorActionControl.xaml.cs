using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Graphics;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Graphics;

[MappingControl(
    ForType = typeof(SetCursorAction),
    ImageResourceName = "ms-appx:///Assets/Icons/cursor.png"
)]
public sealed partial class SetCursorActionControl : UserControl
{
    public SetCursorActionControl()
    {
        this.InitializeComponent();
    }
}
