using System;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping.Triggers.Logic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers.Logic;

[MappingControl(
    ForType = typeof(CombinedTrigger),
    TextGlyph = "\uE71B"
)]
public sealed partial class CombinedTriggerControl : UserControl, ITriggerOptionsControl
{
    public event EventHandler? OptionsChanged;

    public CombinedTriggerControl()
        => this.InitializeComponent();

    private CombinedTriggerControlItem AddTriggerControl(AbstractTrigger? trigger = null)
    {
        var item = new CombinedTriggerControlItem
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        item.RequestedRemove += (s, _) =>
        {
            this.TriggersPanel.Children.Remove(s as UIElement);
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        };

        item.TriggerChanged += (s, _) =>
            OptionsChanged?.Invoke(this, EventArgs.Empty);

        this.TriggersPanel.Children.Add(item);

        if (trigger != null)
        {
            item.SetTrigger(trigger);
        }

        return item;
    }

    void ITriggerOptionsControl.Select(AbstractTrigger combinedTrigger)
    {
        var thisTrigger = (CombinedTrigger)combinedTrigger;

        if (thisTrigger.Triggers == null)
        {
            return;
        }

        foreach (var trigger in thisTrigger.Triggers)
        {
            this.AddTriggerControl(trigger);
        }
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (CombinedTrigger)trigger;

        thisTrigger.Triggers = [];

        foreach (var child in this.TriggersPanel.Children)
        {
            if (child is CombinedTriggerControlItem item && item.Trigger != null)
            {
                thisTrigger.Triggers.Add(item.Trigger);
            }
        }
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger) => true;

    private void AddTriggerButton_Click(object sender, RoutedEventArgs e)
        => this.AddTriggerControl();
}
