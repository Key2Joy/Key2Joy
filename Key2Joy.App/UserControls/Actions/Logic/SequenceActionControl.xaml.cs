using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

[MappingControl(
    ForType = typeof(SequenceAction),
    ImageResourceName = "ms-appx:///Assets/Icons/text_list_numbers.png"
)]
public sealed partial class SequenceActionControl : UserControl
{
    public SequenceActionControl()
    {
        this.InitializeComponent();
    }
}
