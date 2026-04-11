using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.LowLevelInput;
using Key2Joy.LowLevelInput.SimulatedGamePad;
using Key2Joy.Mapping.Actions.Input;
using Key2Joy.Util;
using Microsoft.UI.Xaml.Controls;
using SimWinInput;

namespace Key2Joy.App.UserControls.Actions.Input;

[MappingControl(
    ForType = typeof(GamePadButtonAction),
    ImageResourceName = "ms-appx:///Assets/Icons/joystick.png"
)]
[ObservableObject]
public sealed partial class GamePadActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<GamePadControl> AvailableButtons { get; } = [];
    public ObservableCollection<PressState> AvailablePressStates { get; } = [];
    public ObservableCollection<int> AvailableGamePadIndices { get; } = [];

    [ObservableProperty]
    public partial GamePadControl SelectedButton { get; set; }

    [ObservableProperty]
    public partial PressState SelectedPressState { get; set; }

    [ObservableProperty]
    public partial int SelectedGamePadIndex { get; set; }

    public GamePadActionControl()
    {
        this.InitializeComponent();
        this.LoadButtons();
        this.LoadPressStates();
        this.LoadGamePadIndices();
    }

    private void LoadButtons()
    {
        this.AvailableButtons.Clear();
        foreach (var button in GamePadButtonAction.GetAllButtons())
        {
            this.AvailableButtons.Add(button);
        }
        if (this.AvailableButtons.Count > 0)
        {
            this.SelectedButton = this.AvailableButtons[0];
        }
    }

    private void LoadPressStates()
    {
        this.AvailablePressStates.Clear();
        foreach (var state in PressStates.ALL)
        {
            this.AvailablePressStates.Add(state);
        }
        this.SelectedPressState = this.AvailablePressStates[0];
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

    partial void OnSelectedButtonChanged(GamePadControl value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedPressStateChanged(PressState value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedGamePadIndexChanged(int value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (GamePadButtonAction)action;
        this.SelectedButton = thisAction.Control;
        this.SelectedPressState = thisAction.PressState;
        this.SelectedGamePadIndex = thisAction.GamePadIndex;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (GamePadButtonAction)action;
        thisAction.Control = this.SelectedButton;
        thisAction.PressState = this.SelectedPressState;
        thisAction.GamePadIndex = this.SelectedGamePadIndex;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action)
    {
        var thisAction = (GamePadButtonAction)action;
        if (thisAction.Control != GamePadControl.None)
        {
            return true;
        }

        _ = this.ShowCannotSaveDialogAsync();
        return false;
    }

    private async Task ShowCannotSaveDialogAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Cannot save!",
            Content = "No GamePad button has been selected.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
        };
        await dialog.ShowAsync();
    }
}
