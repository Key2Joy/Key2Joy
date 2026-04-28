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
    ForType = typeof(GamePadTriggerAction),
    TextGlyph = "\uF10A"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class GamePadTriggerActionControl : UserControl, IActionOptionsControl
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
    public partial double InputScale { get; set; } = 1;

    [ObservableProperty]
    public partial double ExactDelta { get; set; } = 0;

    public GamePadTriggerActionControl()
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

    partial void OnInputScaleChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnExactDeltaChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (GamePadTriggerAction)action;
        this.SelectedSide = thisAction.Side;
        this.SelectedGamePadIndex = thisAction.GamePadIndex;
        this.IsInputMode = thisAction.Delta == null;
        this.InputScale = thisAction.InputScale;
        this.ExactDelta = thisAction.Delta ?? 0;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (GamePadTriggerAction)action;
        thisAction.Side = this.SelectedSide;
        thisAction.GamePadIndex = this.SelectedGamePadIndex;
        thisAction.InputScale = (float)this.InputScale;
        thisAction.Delta = this.IsInputMode ? null : (float?)this.ExactDelta;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;
}
