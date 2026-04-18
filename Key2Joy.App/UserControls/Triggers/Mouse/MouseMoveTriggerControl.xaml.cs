using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Mapping.Triggers.Mouse;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers.Mouse;

[MappingControl(
    ForType = typeof(MouseMoveTrigger),
    ImageResourceName = "ms-appx:///Assets/Icons/mouse.png"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class MouseMoveTriggerControl : UserControl, ITriggerOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<AxisDirection> AvailableAxisDirections { get; } = [];

    [ObservableProperty]
    public partial AxisDirection SelectedAxisDirection { get; set; }

    public MouseMoveTriggerControl()
    {
        this.InitializeComponent();
        this.LoadAxisDirections();
    }

    private void LoadAxisDirections()
    {
        this.AvailableAxisDirections.Clear();
        foreach (var direction in Enum.GetValues<AxisDirection>())
        {
            this.AvailableAxisDirections.Add(direction);
        }
        this.SelectedAxisDirection = this.AvailableAxisDirections[0];
    }

    partial void OnSelectedAxisDirectionChanged(AxisDirection value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    void ITriggerOptionsControl.Select(AbstractTrigger trigger)
    {
        var thisTrigger = (MouseMoveTrigger)trigger;
        this.SelectedAxisDirection = thisTrigger.AxisBinding;
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (MouseMoveTrigger)trigger;
        thisTrigger.AxisBinding = this.SelectedAxisDirection;
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger) => true;
}
