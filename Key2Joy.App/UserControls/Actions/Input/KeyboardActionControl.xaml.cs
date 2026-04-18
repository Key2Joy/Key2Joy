using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.LowLevelInput;
using Key2Joy.Mapping.Actions.Input;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Input;

[MappingControl(
    ForType = typeof(KeyboardAction),
    ImageResourceName = "ms-appx:///Assets/Icons/keyboard.png"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class KeyboardActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<KeyboardKey> AvailableKeys { get; } = [];
    public ObservableCollection<PressState> AvailablePressStates { get; } = [];

    [ObservableProperty]
    public partial KeyboardKey SelectedKey { get; set; }

    [ObservableProperty]
    public partial PressState SelectedPressState { get; set; }

    public KeyboardActionControl()
    {
        this.InitializeComponent();
        this.LoadKeys();
        this.LoadPressStates();
    }

    private void LoadKeys()
    {
        this.AvailableKeys.Clear();
        foreach (var key in KeyboardAction.GetAllKeys())
        {
            this.AvailableKeys.Add(key);
        }
        if (this.AvailableKeys.Count > 0)
        {
            this.SelectedKey = this.AvailableKeys[0];
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

    partial void OnSelectedKeyChanged(KeyboardKey value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    partial void OnSelectedPressStateChanged(PressState value)
        => OptionsChanged?.Invoke(this, EventArgs.Empty);

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (KeyboardAction)action;
        this.SelectedKey = thisAction.Key;
        this.SelectedPressState = thisAction.PressState;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (KeyboardAction)action;
        thisAction.Key = this.SelectedKey;
        thisAction.PressState = this.SelectedPressState;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;
}
