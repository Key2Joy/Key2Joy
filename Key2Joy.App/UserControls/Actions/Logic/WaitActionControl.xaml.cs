using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

[MappingControl(
    ForType = typeof(WaitAction),
    ImageResourceName = "ms-appx:///Assets/Icons/clock.png"
)]
[ObservableObject]
public sealed partial class WaitActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;

    [ObservableProperty]
    public partial double WaitTimeMs { get; set; } = 1000;

    public WaitActionControl()
        => this.InitializeComponent();

    partial void OnWaitTimeMsChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (WaitAction)action;
        this.WaitTimeMs = thisAction.WaitTime.TotalMilliseconds;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (WaitAction)action;
        thisAction.WaitTime = TimeSpan.FromMilliseconds(this.WaitTimeMs);
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;
}
