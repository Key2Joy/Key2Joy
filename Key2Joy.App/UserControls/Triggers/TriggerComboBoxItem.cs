using System;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping;

namespace Key2Joy.App.UserControls.Triggers;

public class TriggerComboBoxItem
{
    public TriggerAttribute? TriggerAttribute { get; set; }
    public MappingTypeFactory<AbstractTrigger>? TypeFactory { get; set; }
    public MappingControlFactory? MappingControlFactory { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUri { get; set; }
}
