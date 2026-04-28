using System;
using System.Collections.Generic;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Mapping.Triggers.Mouse;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.Pages;

public sealed partial class MainMappingPage : Page, IAcceptAppCommands, IInvokeOnUI
{
    public MainMappingPageViewModel ViewModel { get; } = new();

    public MainMappingPage() => this.InitializeComponent();

    private void RootGrid_Loaded(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Initialize();

        // Ensure the manager knows which window handle catches all inputs
        Key2JoyManager.Instance.SetHandlerWithInvoke(this);
        Key2JoyManager.Instance.StatusChanged += (s, ev) =>
        {
            if (ev.Profile != null)
            {
                this.SetSelectedProfile(ev.Profile);
            }
        };

        this.MappingControl.MappingCreated += this.MappingControl_MappingCreated;
        this.MappingControl.MappingDeleted += this.MappingControl_MappingDeleted;

        this.MappingGroupsList.EditMappingRequested += this.MappingGroupsList_EditMappingRequested;
        this.MappingGroupsList.GenerateReversesRequested += this.MappingGroupsList_GenerateReversesRequested;
        this.MappingGroupsList.RemoveMappingRequested += this.MappingGroupsList_RemoveMappingRequested;
        this.MappingGroupsList.MakeMappingParentlessRequested += this.MappingGroupsList_MakeMappingParentlessRequested;
        this.MappingGroupsList.ChooseNewParentRequested += this.MappingGroupsList_ChooseNewParentRequested;
    }

    private void MappingGroupsList_EditMappingRequested(object? sender, MappedOption? mappedOption)
    {
        this.ViewModel.SelectMapping(mappedOption);
        this.MappingControl.SelectMapping(mappedOption);
        this.ViewModel.IsDrawerOpen = true;
    }

    private async void MappingGroupsList_GenerateReversesRequested(object? sender, IList<MappedOption> mappedOptions)
    {
        if (mappedOptions == null || mappedOptions.Count == 0)
        {
            return;
        }

        if (mappedOptions.Count > 1)
        {
            var dialog = new ContentDialog
            {
                Title = $"Generate {mappedOptions.Count} reverse mappings",
                Content = $"Are you sure you want to create reverse mappings for all {mappedOptions.Count} selected mappings? "
                    + "Each type of action and trigger will configure their own useful reverse if possible.\n\n"
                    + "An example of a reverse mapping is how new 'Release' mappings will be created for each 'Press' and vice versa.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;
            }
        }

        this.ViewModel.GenerateReversesForMappings(mappedOptions);
    }

    private void MappingGroupsList_RemoveMappingRequested(object? sender, MappedOption? mappedOption)
    {
        if (mappedOption == null)
        {
            return;
        }

        this.MappingControl.SelectMapping(null);
        this.ViewModel.HandleMappingDeleted(mappedOption);
    }

    private void MappingGroupsList_MakeMappingParentlessRequested(object? sender, MappedOption mappedOption)
    {
        this.ViewModel.MakeMappingParentless(mappedOption);
        this.DeselectSelectedMapping();
    }

    private void MappingGroupsList_ChooseNewParentRequested(object? sender, (MappedOption Child, MappedOption NewParent) args)
    {
        this.ViewModel.ChooseNewParent(args.Child, args.NewParent);
        this.DeselectSelectedMapping();
    }

    private void MappingControl_MappingCreated(object? sender, MappedOption mappedOption)
    {
        this.ViewModel.HandleMappingCreated(mappedOption);
        this.ViewModel.IsDrawerOpen = false;
        this.DeselectSelectedMapping();
    }

    private void MappingControl_MappingDeleted(object? sender, MappedOption mappedOption)
    {
        this.ViewModel.HandleMappingDeleted(mappedOption);
        this.ViewModel.IsDrawerOpen = false;
        this.DeselectSelectedMapping();
    }

    private void MappingGroupsList_MappingSelected(object sender, MappedOption option)
    {
        this.ViewModel.SelectMapping(option);
        this.MappingControl.SelectMapping(option);
    }

    private void NewMappingFab_Click(object sender, EventArgs e) => this.OpenDrawerForNewMapping();

    public void OpenDrawerForNewMapping()
    {
        this.ViewModel.SelectMapping(null);
        this.MappingControl.SelectMapping(null);
        this.ViewModel.IsDrawerOpen = true;
    }

    private void MappingDrawer_Closed(object sender, EventArgs e)
    {
        this.ViewModel.SelectMapping(null);
        this.MappingGroupsList.ClearSelection();
        this.MappingControl.SelectMapping(null);
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

    public MappingProfile CreateNewProfile(string? nameSuffix = default)
        => this.ViewModel.CreateNewProfile(nameSuffix);

    public void DeselectSelectedMapping()
    {
        this.ViewModel.DeselectSelectedMapping();
        this.MappingGroupsList.ClearSelection();
        // We explicitly do not call this, as it would cause the MappingControl to reset, which would be jarringly visible for users
        // with animations enabled:
        // this.MappingControl.SelectMapping(null);
        this.ViewModel.IsDrawerOpen = false;
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
        this.ViewModel.IsDrawerOpen = false;
    }

    public void ExpandAllMappings() => this.MappingGroupsList.ExpandAll();

    public void CollapseAllMappings() => this.MappingGroupsList.CollapseAll();


    public void SetSelectedProfile(MappingProfile profile)
        => this.ViewModel.SetSelectedProfile(profile);

    public void RefreshMappingList()
        => this.ViewModel.RefreshMappingList();

    public object Invoke(Delegate method)
        => this.DispatcherQueue.TryEnqueue(() => method.DynamicInvoke());

    public object Invoke(Delegate method, params object[] arguments)
        => this.DispatcherQueue.TryEnqueue(() => method.DynamicInvoke(arguments));
}
