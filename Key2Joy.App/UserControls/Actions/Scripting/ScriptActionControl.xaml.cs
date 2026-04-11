using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Actions.Scripting;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Scripting;

[MappingControl(
    ForType = typeof(LuaScriptAction),
    ImageResourceName = "ms-appx:///Assets/Icons/script_code.png"
)]
public sealed partial class ScriptActionControl : UserControl
{
    public ScriptActionControl()
    {
        this.InitializeComponent();
    }
}
