using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

[MappingControl(
    ForType = typeof(AppCommandAction),
    ImageResourceName = "ms-appx:///Assets/Icons/application_xp_terminal.png"
)]
[ObservableObject]
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
