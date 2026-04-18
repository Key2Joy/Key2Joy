using System;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping;

namespace Key2Joy.App.UserControls.Actions;


public class ActionComboBoxItem
{
    public ActionAttribute? ActionAttribute { get; set; }
    public MappingTypeFactory<AbstractAction>? TypeFactory { get; set; }
    public MappingControlFactory? MappingControlFactory { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUri { get; set; }
}
