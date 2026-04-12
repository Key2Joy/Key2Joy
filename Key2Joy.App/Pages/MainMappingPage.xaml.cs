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
        this.MappingControl.SelectMapping(null);
    }

    private void MappingControl_MappingDeleted(object sender, Key2Joy.Mapping.MappedOption mappedOption)
    {
        this.ViewModel.HandleMappingDeleted(mappedOption);
        this.MappingControl.SelectMapping(null);
    }

    private void MappingControl_MappingDeselected(object sender, Key2Joy.Mapping.MappedOption mappedOption)
    {
        this.ViewModel.SelectMapping(null);
        this.MappingControl.SelectMapping(null);
    }

    private void MappingGroupsList_MappingSelected(object sender, MappedOption option)
    {
        this.ViewModel.SelectMapping(option);
        this.MappingControl.SelectMapping(option);
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

    public void DeselectSelectedMapping()
    {
        this.ViewModel.DeselectSelectedMapping();
        this.MappingControl.SelectMapping(null);
    }

    public void DeleteSelectedMapping()
    {
        var option = this.ViewModel.GetSelectedMappingOption();

        if (option == null)
        {
            return;
        }

        this.MappingControl.SelectMapping(null);
        this.ViewModel.HandleMappingDeleted(option);
    }

    public void ExpandAllMappings() => this.MappingGroupsList.ExpandAll();

    public void CollapseAllMappings() => this.MappingGroupsList.CollapseAll();


    public void SetSelectedProfile(MappingProfile profile)
        => this.ViewModel.SetSelectedProfile(profile);
}
