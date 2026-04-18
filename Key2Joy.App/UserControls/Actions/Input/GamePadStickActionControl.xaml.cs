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
    ImageResourceName = "ms-appx:///Assets/Icons/joystick.png"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class GamePadStickActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<GamePadSide> AvailableSides { get; } = [];
    public ObservableCollection<int> AvailableGamePadIndices { get; } = [];

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
    public partial double ExactX { get; set; } = 0;

    [ObservableProperty]
    public partial double ExactY { get; set; } = 0;

    [ObservableProperty]
    public partial double ResetAfterMs { get; set; } = 500;

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
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    partial void OnInputScaleXChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnInputScaleYChanged(double value)
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
        this.ResetAfterMs = thisAction.ResetAfterIdleTimeInMs;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (GamePadStickAction)action;
        thisAction.Side = this.SelectedSide;
        thisAction.GamePadIndex = this.SelectedGamePadIndex;
        thisAction.InputScaleX = (float)this.InputScaleX;
        thisAction.InputScaleY = (float)this.InputScaleY;
        thisAction.DeltaX = this.IsInputMode ? null : (short?)this.ExactX;
        thisAction.DeltaY = this.IsInputMode ? null : (short?)this.ExactY;
        thisAction.ResetAfterIdleTimeInMs = (int)this.ResetAfterMs;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;
}
