using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping.Actions.Input;
using Key2Joy.Mapping.Triggers.GamePad;
using Key2Joy.Util;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Input;

[MappingControl(
    ForType = typeof(GamePadStickAction),
    TextGlyph = "\uF108"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class GamePadStickActionControl : UserControl, IActionOptionsControl, IReverseMappingAwareOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<GamePadSide> AvailableSides { get; } = [];
    public ObservableCollection<int> AvailableGamePadIndices { get; } = [];
    public ObservableCollection<GamePadStickInputMode> AvailableInputModes { get; } = [.. Enum.GetValues<GamePadStickInputMode>()];

    [ObservableProperty]
    public partial GamePadSide SelectedSide { get; set; }

    [ObservableProperty]
    public partial int SelectedGamePadIndex { get; set; }

    [ObservableProperty]
    public partial bool IsInputMode { get; set; } = true;

    public bool IsExactMode => !this.IsInputMode;

    [ObservableProperty]
    public partial double InputScaleX { get; set; } = 1;

    [ObservableProperty]
    public partial double InputScaleY { get; set; } = -1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSpeedSettingsVisible), nameof(IsResetAfterVisible))]
    public partial GamePadStickInputMode SelectedInputMode { get; set; } = GamePadStickInputMode.Accumulate;

    [ObservableProperty]
    public partial double SpeedFullDeflection { get; set; } = 2000;

    [ObservableProperty]
    public partial double SpeedSmoothingMs { get; set; } = 20;

    [ObservableProperty]
    public partial double SpeedCurve { get; set; } = 1;

    [ObservableProperty]
    public partial double SpeedMinimumOutput { get; set; } = 0;

    public bool IsSpeedSettingsVisible => this.IsInputMode && this.SelectedInputMode == GamePadStickInputMode.Speed;

    /// <summary>
    /// Speed mode returns the stick by itself, so it doesn't use the reset.
    /// </summary>
    public bool IsResetAfterVisible => !this.IsSpeedSettingsVisible;

    [ObservableProperty]
    public partial double ExactX { get; set; } = 0;

    [ObservableProperty]
    public partial double ExactY { get; set; } = 0;

    [ObservableProperty]
    public partial double ResetAfterMs { get; set; } = 500;

    /// <summary>
    /// When the mapping also creates a reverse mapping (e.g. on key release), that
    /// reverse returns the stick. A reset would center the stick while the key is still
    /// held, so it's not used.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsResetAfterAvailable))]
    public partial bool CreatesReverseMapping { get; set; }

    public bool IsResetAfterAvailable => !this.CreatesReverseMapping;

    public GamePadStickActionControl()
    {
        this.InitializeComponent();
        this.LoadSides();
        this.LoadGamePadIndices();
    }

    private void LoadSides()
    {
        this.AvailableSides.Clear();
        foreach (var side in Enum.GetValues<GamePadSide>())
        {
            this.AvailableSides.Add(side);
        }
        this.SelectedSide = this.AvailableSides[0];
    }

    private void LoadGamePadIndices()
    {
        this.AvailableGamePadIndices.Clear();
        try
        {
            var gamePadService = ServiceContainer.Get<ISimulatedGamePadService>();
            foreach (var gamePad in gamePadService.GetAllGamePads(false))
            {
                this.AvailableGamePadIndices.Add(gamePad.Index);
            }
        }
        catch { }

        if (this.AvailableGamePadIndices.Count == 0)
        {
            this.AvailableGamePadIndices.Add(0);
        }
        this.SelectedGamePadIndex = this.AvailableGamePadIndices[0];
    }

    partial void OnSelectedSideChanged(GamePadSide value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedGamePadIndexChanged(int value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnIsInputModeChanged(bool value)
    {
        this.OnPropertyChanged(nameof(IsExactMode));
        this.OnPropertyChanged(nameof(IsSpeedSettingsVisible));
        this.OnPropertyChanged(nameof(IsResetAfterVisible));
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnInputScaleXChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnInputScaleYChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedInputModeChanged(GamePadStickInputMode value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSpeedFullDeflectionChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSpeedSmoothingMsChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSpeedCurveChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSpeedMinimumOutputChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnExactXChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnExactYChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnResetAfterMsChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (GamePadStickAction)action;
        this.SelectedSide = thisAction.Side;
        this.SelectedGamePadIndex = thisAction.GamePadIndex;
        this.IsInputMode = thisAction.DeltaX == null && thisAction.DeltaY == null;
        this.InputScaleX = thisAction.InputScaleX;
        this.InputScaleY = thisAction.InputScaleY;
        this.ExactX = thisAction.DeltaX ?? 0;
        this.ExactY = thisAction.DeltaY ?? 0;
        this.SelectedInputMode = thisAction.InputMode;
        this.SpeedFullDeflection = thisAction.SpeedFullDeflection;
        this.SpeedSmoothingMs = thisAction.SpeedSmoothingMs;
        this.SpeedCurve = thisAction.SpeedCurve;
        this.SpeedMinimumOutput = thisAction.SpeedMinimumOutput;
        this.ResetAfterMs = thisAction.ResetAfterIdleTimeInMs;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (GamePadStickAction)action;
        thisAction.Side = this.SelectedSide;
        thisAction.GamePadIndex = this.SelectedGamePadIndex;
        thisAction.InputScaleX = (float)this.InputScaleX;
        thisAction.InputScaleY = (float)this.InputScaleY;
        thisAction.DeltaX = this.IsInputMode ? null : (float)this.ExactX;
        thisAction.DeltaY = this.IsInputMode ? null : (float)this.ExactY;
        thisAction.InputMode = this.SelectedInputMode;
        thisAction.SpeedFullDeflection = (float)this.SpeedFullDeflection;
        thisAction.SpeedSmoothingMs = (int)this.SpeedSmoothingMs;
        thisAction.SpeedCurve = (float)this.SpeedCurve;
        thisAction.SpeedMinimumOutput = (float)this.SpeedMinimumOutput;
        // 0 means never reset
        thisAction.ResetAfterIdleTimeInMs = this.CreatesReverseMapping ? 0 : (int)this.ResetAfterMs;
    }

    void IReverseMappingAwareOptionsControl.SetCreatesReverseMapping(bool createsReverseMapping)
        => this.CreatesReverseMapping = createsReverseMapping;

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;
}
