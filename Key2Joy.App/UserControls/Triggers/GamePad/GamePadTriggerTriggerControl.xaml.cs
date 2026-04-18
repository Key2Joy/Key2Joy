using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Mapping.Triggers.GamePad;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers.GamePad;

[MappingControl(
    ForType = typeof(GamePadTriggerTrigger),
    ImageResourceName = "ms-appx:///Assets/Icons/joystick.png"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class GamePadTriggerTriggerControl : UserControl, ITriggerOptionsControl
{
    public event EventHandler? OptionsChanged;

    public ObservableCollection<GamePadSide> AvailableTriggerSides { get; } = [];

    [ObservableProperty]
    public partial GamePadSide SelectedTriggerSide { get; set; }

    [ObservableProperty]
    public partial double GamePadIndex { get; set; } = 0;

    [ObservableProperty]
    public partial bool OverrideDeadzone { get; set; } = false;

    [ObservableProperty]
    public partial double Deadzone { get; set; } = 0;

    public GamePadTriggerTriggerControl()
    {
        this.InitializeComponent();

        foreach (var side in Enum.GetValues<GamePadSide>())
        {
            this.AvailableTriggerSides.Add(side);
        }

        this.SelectedTriggerSide = this.AvailableTriggerSides[0];
    }

    void ITriggerOptionsControl.Select(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadTriggerTrigger)trigger;

        this.GamePadIndex = thisTrigger.GamePadIndex;
        this.SelectedTriggerSide = thisTrigger.TriggerSide;
        this.OverrideDeadzone = thisTrigger.DeltaMargin != null;
        this.Deadzone = thisTrigger.DeltaMargin ?? 0;
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadTriggerTrigger)trigger;

        thisTrigger.GamePadIndex = (int)this.GamePadIndex;
        thisTrigger.TriggerSide = this.SelectedTriggerSide;
        thisTrigger.DeltaMargin = this.OverrideDeadzone ? (float)this.Deadzone : null;
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger) => true;

    partial void OnSelectedTriggerSideChanged(GamePadSide value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnGamePadIndexChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnOverrideDeadzoneChanged(bool value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnDeadzoneChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);
}
