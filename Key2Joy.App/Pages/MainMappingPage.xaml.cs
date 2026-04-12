using System;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Mapping.Triggers.Mouse;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.Pages;

public sealed partial class MainMappingPage : Page, IAcceptAppCommands
{
    public MainMappingPageViewModel ViewModel { get; } = new();

    public MainMappingPage()
    {
        this.InitializeComponent();
    }

    private void RootGrid_Loaded(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Initialize();

        this.MappingControl.MappingCreated += this.MappingControl_MappingCreated;
        this.MappingControl.MappingDeleted += this.MappingControl_MappingDeleted;
        this.MappingControl.MappingDeselected += this.MappingControl_MappingDeselected;
    }

    private void MappingControl_MappingCreated(object sender, Key2Joy.Mapping.MappedOption mappedOption)
    {
        this.ViewModel.HandleMappingCreated(mappedOption);
        this.MappingListView.SelectedItem = null;
    }

    private void MappingControl_MappingDeleted(object sender, Key2Joy.Mapping.MappedOption mappedOption)
    {
        this.ViewModel.HandleMappingDeleted(mappedOption);
        this.MappingListView.SelectedItem = null;
    }

    private void MappingControl_MappingDeselected(object sender, Key2Joy.Mapping.MappedOption mappedOption)
    {
        this.MappingListView.SelectedItem = null;
    }

    private void MappingListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listView = sender as ListView;
        var selected = listView.SelectedItem as Key2Joy.Mapping.MappedOption;
        this.MappingControl.SelectMapping(selected);
    }

    public bool RunAppCommand(AppCommand command)
    {
        switch (command)
        {
            case AppCommand.Abort:
                this.ViewModel.Armed = false;

                return true;

            case AppCommand.ResetScriptEnvironment:
                /// Handled in <see cref="AppCommandAction.ExecuteForScript"/>
                /// TODO: Handle it here as well?
                break;

            case AppCommand.ResetMouseMoveTriggerCenter:
                /// Also handled in <see cref="AppCommandAction.ExecuteForScript"/>
                /// TODO: Remove duplicate code
                MouseMoveTriggerListener.Instance.ResetCenterCursor();
                return true;

            default:
                break;
        }

        return false;
    }

    public MappingProfile CreateNewProfile(string nameSuffix = default)
        => this.ViewModel.CreateNewProfile(nameSuffix);


    public void SetSelectedProfile(MappingProfile profile)
        => this.ViewModel.SetSelectedProfile(profile);
}
