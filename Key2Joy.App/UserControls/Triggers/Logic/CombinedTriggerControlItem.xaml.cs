using System;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping.Triggers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers.Logic;

public sealed partial class CombinedTriggerControlItem : UserControl
{
    public event EventHandler RequestedRemove;

    public event EventHandler TriggerChanged;

    public AbstractTrigger Trigger => this.triggerControl.Trigger;

    public CombinedTriggerControlItem()
    {
        this.InitializeComponent();

        this.triggerControl.IsTopLevel = false;
        this.triggerControl.TriggerChanged += this.TriggerControl_TriggerChanged;
    }

    public void SetTrigger(AbstractTrigger trigger)
    {
        this.Loaded += OnLoadedSelectTrigger;

        void OnLoadedSelectTrigger(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoadedSelectTrigger;
            this.triggerControl.SelectTrigger(trigger);
        }
    }

    private void TriggerControl_TriggerChanged(object sender, TriggerChangedEventArgs e)
        => TriggerChanged?.Invoke(this, EventArgs.Empty);

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
        => RequestedRemove?.Invoke(this, EventArgs.Empty);
}
