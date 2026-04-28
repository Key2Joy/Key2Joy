using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

[MappingControl(
    ForType = typeof(AppCommandAction),
    TextGlyph = "\uE756"
)]
[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class AppCommandActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;
    public ObservableCollection<AppCommand> AvailableAppCommands { get; } = [];

    [ObservableProperty]
    public partial AppCommand SelectedAppCommand { get; set; }

    public AppCommandActionControl()
    {
        this.InitializeComponent();

        this.LoadAppCommands();
    }


    private void LoadAppCommands()
    {
        this.AvailableAppCommands.Clear();

        foreach (var command in Enum.GetValues<AppCommand>())
        {
            this.AvailableAppCommands.Add(command);
        }
    }

    partial void OnSelectedAppCommandChanged(AppCommand value)
    {
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (AppCommandAction)action;

        this.SelectedAppCommand = thisAction.Command;
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (AppCommandAction)action;

        thisAction.Command = this.SelectedAppCommand;
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action)
        => true;
}
