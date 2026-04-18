using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.LowLevelInput;
using Key2Joy.LowLevelInput.XInput;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Mapping.Triggers.GamePad;
using Key2Joy.Util;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers.GamePad;

[MappingControl(
    ForType = typeof(GamePadButtonTrigger),
    ImageResourceName = "ms-appx:///Assets/Icons/joystick.png"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class GamePadButtonTriggerControl : UserControl, ITriggerOptionsControl
{
    private const string TEXT_CHANGE = "(press any button to select it)";
    private const string TEXT_CHANGE_INSTRUCTION = "(click to change)";

    public ObservableCollection<PressState> AvailablePressStates { get; } = [];

    [ObservableProperty]
    public partial string ButtonBindText { get; set; } = TEXT_CHANGE_INSTRUCTION;

    [ObservableProperty]
    public partial string LastGamePadLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial PressState SelectedPressState { get; set; }

    [ObservableProperty]
    public partial double GamePadIndex { get; set; } = 0;

    private readonly CompositeFormat textLastGamepad = CompositeFormat.Parse("Last GamePad used was #{0}");

    public event EventHandler? OptionsChanged;

    private readonly IXInputService xInputService;
    private GamePadButton button;

    public GamePadButtonTriggerControl()
    {
        this.InitializeComponent();

        this.xInputService = ServiceContainer.Get<IXInputService>();
        this.xInputService.StateChanged += this.XInputService_StateChanged;

        foreach (var state in PressStates.ALL)
        {
            this.AvailablePressStates.Add(state);
        }

        this.SelectedPressState = PressStates.ALL[0];

        this.Unloaded += (s, e) =>
        {
            this.xInputService.StateChanged -= this.XInputService_StateChanged;
            this.xInputService.StopPolling();
        };
    }

    private void XInputService_StateChanged(object? sender, DeviceStateChangedEventArgs e)
    {
        var buttons = e.NewState.Gamepad.GetPressedButtonsList();

        if (buttons.Count == 0)
        {
            return;
        }

        this.button = buttons.First();

        this.DispatcherQueue.TryEnqueue(() =>
        {
            this.LastGamePadLabel = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                this.textLastGamepad,
                e.DeviceIndex
            );
            this.UpdateButtonDisplay();
            this.StopTrapping();
        });
    }

    void ITriggerOptionsControl.Select(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadButtonTrigger)trigger;

        this.button = thisTrigger.Button;
        this.SelectedPressState = thisTrigger.PressState;
        this.UpdateButtonDisplay();
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadButtonTrigger)trigger;

        thisTrigger.Button = this.button;
        thisTrigger.PressState = this.SelectedPressState;
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger)
    {
        var thisTrigger = (GamePadButtonTrigger)trigger;

        if (thisTrigger.Button != 0)
        {
            return true;
        }

        _ = this.ShowErrorAsync("The trigger is not set to any button.");
        return false;
    }

    private void StartTrapping()
    {
        this.ButtonBindText = TEXT_CHANGE;
        this.xInputService.RecognizePhysicalDevices();
        this.xInputService.StartPolling();
    }

    private void StopTrapping()
    {
        this.xInputService.StopPolling();
        // Shift focus away from the text box so it can be clicked again
        this.UnfocusTextBox.Focus(FocusState.Programmatic);
    }

    private void UpdateButtonDisplay()
    {
        this.ButtonBindText = $"{this.button} {TEXT_CHANGE_INSTRUCTION}";
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    private async System.Threading.Tasks.Task ShowErrorAsync(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Cannot save!",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private void ButtonBindTextBox_GotFocus(object sender, RoutedEventArgs e)
        => this.StartTrapping();

    partial void OnSelectedPressStateChanged(PressState value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnGamePadIndexChanged(double value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);
}
