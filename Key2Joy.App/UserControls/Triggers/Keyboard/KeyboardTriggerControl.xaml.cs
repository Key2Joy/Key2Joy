using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.LowLevelInput;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Mapping.Triggers.Keyboard;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Key2Joy.App.UserControls.Triggers.Keyboard;

[MappingControl(
    ForType = typeof(KeyboardTrigger),
    TextGlyph = "\uE765"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class KeyboardTriggerControl : UserControl, ITriggerOptionsControl, IDisposable
{
    private const string TEXT_CHANGE = "(press any key to select it as the trigger)";
    private const string TEXT_CHANGE_INSTRUCTION = "(click here, then press any key to set it as the trigger)";
    private readonly VirtualKeyConverter virtualKeyConverter = new();

    public event EventHandler? OptionsChanged;
    public ObservableCollection<PressState> AvailablePressStates { get; } = [];

    [ObservableProperty]
    public partial string KeyBindText { get; set; } = TEXT_CHANGE_INSTRUCTION;

    [ObservableProperty]
    public partial PressState SelectedPressState { get; set; }

    private System.Windows.Forms.Keys keys;
    private bool isTrapping;
    private GlobalInputHook? globalKeyboardHook;

    public KeyboardTriggerControl()
    {
        this.InitializeComponent();

        this.LoadPressStates();

        this.globalKeyboardHook = new GlobalInputHook();
        this.globalKeyboardHook.KeyboardInputEvent += this.OnKeyInputEvent;

        // Without this we run into an exception. Seems like Dispose for this User Control is not called.
        this.Unloaded += (s, e) => this.CleanupKeyboardHook();
    }

    private void CleanupKeyboardHook()
    {
        if (this.globalKeyboardHook == null)
        {
            return;
        }

        this.globalKeyboardHook.KeyboardInputEvent -= this.OnKeyInputEvent;
        this.globalKeyboardHook.Dispose();
        this.globalKeyboardHook = null;
        this.UnblockKeyboardAccelerators();
    }

    private void LoadPressStates()
    {
        this.AvailablePressStates.Clear();

        foreach (var pressState in PressStates.ALL)
        {
            this.AvailablePressStates.Add(pressState);
        }

        this.SelectedPressState = this.AvailablePressStates[0];
    }

    private void OnKeyInputEvent(object? sender, GlobalKeyboardHookEventArgs e)
    {
        if (!this.isTrapping)
        {
            return;
        }

        this.keys = this.virtualKeyConverter.KeysFromVirtual(e.KeyboardData.VirtualCode);
        this.UpdateKeys();
        this.StopTrapping();
    }

    partial void OnSelectedPressStateChanged(PressState value)
    {
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    void ITriggerOptionsControl.Select(AbstractTrigger trigger)
    {
        var thisTrigger = (KeyboardTrigger)trigger;

        this.keys = thisTrigger.Keys;
        this.SelectedPressState = thisTrigger.PressState;
        this.UpdateKeys();
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (KeyboardTrigger)trigger;

        thisTrigger.Keys = this.keys;
        thisTrigger.PressState = this.SelectedPressState;
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger)
    {
        var thisTrigger = (KeyboardTrigger)trigger;

        if (thisTrigger.Keys != System.Windows.Forms.Keys.None)
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
            Content = "The trigger is not set to any key.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
        };

        await dialog.ShowAsync();
    }

    private void StartTrapping()
    {
        this.KeyBindText = TEXT_CHANGE;
        this.isTrapping = true;
        this.BlockKeyboardAccelerators();
    }

    private void StopTrapping()
    {
        this.isTrapping = false;

        // TODO: Somehow lose focus from the binding textbox without an invisible UnfocusTextBox?
        // We lose focus so the user can just click it again to change the keybind without having to first focus something else
        this.UnfocusTextBox.Focus(FocusState.Programmatic);

        // Delay unblocking by 1s: even deferring to the next dispatcher tick is not enough because
        // the key-up event still passes through WinUI's accelerator pipeline and would trigger
        // menu items (e.g. Escape closing the mapping drawer).
        Task.Delay(1000).ContinueWith(_ => this.DispatcherQueue.TryEnqueue(this.UnblockKeyboardAccelerators));
    }

    private void BlockKeyboardAccelerators()
        => this.ProcessKeyboardAccelerators += this.OnProcessKeyboardAcceleratorsWhileTrapping;

    private void UnblockKeyboardAccelerators()
        => this.ProcessKeyboardAccelerators -= this.OnProcessKeyboardAcceleratorsWhileTrapping;

    private void OnProcessKeyboardAcceleratorsWhileTrapping(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        => args.Handled = true;

    private void UpdateKeys()
    {
        this.KeyBindText = $"{this.keys} {TEXT_CHANGE_INSTRUCTION}";
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void KeyBindTextBox_GotFocus(object sender, RoutedEventArgs e)
        => this.StartTrapping();

    public void Dispose() => this.CleanupKeyboardHook();
}
