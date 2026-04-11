using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Triggers.GamePad;
using Key2Joy.Util;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers.GamePad;

[MappingControl(
    ForType = typeof(GamePadStickTrigger),
    ImageResourceName = "ms-appx:///Assets/Icons/joystick.png"
)]
[ObservableObject]
public sealed partial class GamePadStickTriggerControl : UserControl, ITriggerOptionsControl
{
    public event EventHandler OptionsChanged;

    public ObservableCollection<GamePadSide> AvailableStickSides { get; } = new();

    [ObservableProperty]
    public partial GamePadSide SelectedStickSide { get; set; }

    [ObservableProperty]
    public partial double GamePadIndex { get; set; } = 0;

    [ObservableProperty]
    public partial bool OverrideDeadzone { get; set; } = false;

    [ObservableProperty]
    public partial double DeadzoneX { get; set; } = 0;

    [ObservableProperty]
    public partial double DeadzoneY { get; set; } = 0;

    public GamePadStickTriggerControl()
    {
        this.InitializeComponent();

        foreach (GamePadSide side in Enum.GetValues(typeof(GamePadSide)))
        {
            this.AvailableStickSides.Add(side);
        }

        this.SelectedStickSide = this.AvailableStickSides[0];
    }

    void ITriggerOptionsControl.Select(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadStickTrigger)trigger;

        this.GamePadIndex = thisTrigger.GamePadIndex;
        this.SelectedStickSide = thisTrigger.StickSide;
        this.OverrideDeadzone = thisTrigger.DeltaMargin != null;
        this.DeadzoneX = thisTrigger.DeltaMargin?.X ?? 0;
        this.DeadzoneY = thisTrigger.DeltaMargin?.Y ?? 0;
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadStickTrigger)trigger;

        thisTrigger.GamePadIndex = (int)this.GamePadIndex;
        thisTrigger.StickSide = this.SelectedStickSide;
        thisTrigger.DeltaMargin = this.OverrideDeadzone
            ? new ExactAxisDirection((float)this.DeadzoneX, (float)this.DeadzoneY)
            : null;
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger) => true;

    partial void OnSelectedStickSideChanged(GamePadSide value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnGamePadIndexChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnOverrideDeadzoneChanged(bool value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnDeadzoneXChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnDeadzoneYChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);
}
