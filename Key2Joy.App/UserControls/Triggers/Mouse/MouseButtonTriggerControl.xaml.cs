using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.LowLevelInput;
using Key2Joy.Mapping.Triggers;
using Key2Joy.Mapping.Triggers.Mouse;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Key2Joy.App.UserControls.Triggers.Mouse;

[MappingControl(
    ForType = typeof(MouseButtonTrigger),
    TextGlyph = "\uE962"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class MouseButtonTriggerControl : UserControl, ITriggerOptionsControl, IDisposable
{
    private const string TEXT_HOVER_INSTRUCTION = "(hover here, then click a mouse button to set it as the trigger)";

    public event EventHandler? OptionsChanged;
    public ObservableCollection<PressState> AvailablePressStates { get; } = [];

    [ObservableProperty]
    public partial string ButtonBindText { get; set; } = TEXT_HOVER_INSTRUCTION;

    [ObservableProperty]
    public partial PressState SelectedPressState { get; set; }

    private LowLevelInput.Mouse.Buttons mouseButtons;
    private bool isMouseOver;
    private GlobalInputHook? globalMouseHook;

    public MouseButtonTriggerControl()
    {
        this.InitializeComponent();
        this.LoadPressStates();

        this.globalMouseHook = new GlobalInputHook();
        this.globalMouseHook.MouseInputEvent += this.OnMouseInputEvent;

        // Without this we run into an exception. Seems like Dispose for this User Control is not called.
        this.Unloaded += (s, e) => this.CleanupMouseHook();
    }

    private void CleanupMouseHook()
    {
        if (this.globalMouseHook == null)
        {
            return;
        }

        this.globalMouseHook.MouseInputEvent -= this.OnMouseInputEvent;
        this.globalMouseHook.Dispose();
        this.globalMouseHook = null;
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

    private void OnMouseInputEvent(object? sender, GlobalMouseHookEventArgs e)
    {
        if (!this.isMouseOver)
        {
            return;
        }

        try
        {
            var buttons = LowLevelInput.Mouse.ButtonsFromEvent(e, out var isDown);
            if (!isDown)
            {
                return;
            }

            this.mouseButtons = buttons;
            this.ButtonBindText = $"{this.mouseButtons} {TEXT_HOVER_INSTRUCTION}";
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (NotImplementedException)
        {
            _ = this.ShowUnknownButtonDialogAsync();
        }
    }

    private async Task ShowUnknownButtonDialogAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Unknown button",
            Content = "This mouse button is not supported.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
        };
        await dialog.ShowAsync();
    }

    partial void OnSelectedPressStateChanged(PressState value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    private void ButtonBindTextBox_PointerEntered(object sender, PointerRoutedEventArgs e)
        => this.isMouseOver = true;

    private void ButtonBindTextBox_PointerExited(object sender, PointerRoutedEventArgs e)
        => this.isMouseOver = false;

    void ITriggerOptionsControl.Select(AbstractTrigger trigger)
    {
        var thisTrigger = (MouseButtonTrigger)trigger;
        this.mouseButtons = thisTrigger.MouseButtons;
        this.SelectedPressState = thisTrigger.PressState;
        if (this.mouseButtons != LowLevelInput.Mouse.Buttons.None)
        {
            this.ButtonBindText = $"{this.mouseButtons} {TEXT_HOVER_INSTRUCTION}";
        }
    }

    void ITriggerOptionsControl.Setup(AbstractTrigger trigger)
    {
        var thisTrigger = (MouseButtonTrigger)trigger;
        thisTrigger.MouseButtons = this.mouseButtons;
        thisTrigger.PressState = this.SelectedPressState;
    }

    bool ITriggerOptionsControl.CanMappingSave(AbstractTrigger trigger) => true;

    public void Dispose() => this.CleanupMouseHook();
}
