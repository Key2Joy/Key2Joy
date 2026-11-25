using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Key2Joy.App.UserControls.Actions;
using Key2Joy.App.UserControls.Triggers;
using Key2Joy.Mapping;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls;

[ObservableObject]
[SuppressMessage(
    "CommunityToolkit.Mvvm.SourceGenerators.ObservableObjectGenerator",
    "MVVMTK0050:Using [ObservableObject] is not AOT compatible for WinRT",
    Justification = "Cannot inherit from ObservableObject, must remain UserControl"
)]
public sealed partial class MappingControl : UserControl
{
    [ObservableProperty]
    public partial bool CreateOrUpdateReverseMapping { get; set; }

    [ObservableProperty]
    public partial TriggerComboBoxItem? SelectedTrigger { get; set; }

    [ObservableProperty]
    public partial ActionComboBoxItem? SelectedAction { get; set; }

    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    [ObservableProperty]
    public partial string ModeTitle { get; set; } = "Creating a new mapping";

    [ObservableProperty]
    public partial string SaveButtonText { get; set; } = "Create Mapping";

    public MappedOption? MappedOption { get; private set; }
    public MappedOption? MappedOptionReverse { get; private set; }

    public event EventHandler<MappedOption>? MappingCreated;
    public event EventHandler<MappedOption>? MappingDeleted;

    /// <summary>
    /// Raised (before <see cref="MappingCreated"/>) with the child mappings that should be
    /// removed, because the user unticked creating a reverse mapping.
    /// </summary>
    public event EventHandler<IReadOnlyList<MappedOption>>? ChildMappingsRemoved;

    private bool dominantReverseCheckedState;

    partial void OnCreateOrUpdateReverseMappingChanged(bool value)
        => this.ActionControl.SetCreatesReverseMapping(value);

    partial void OnIsEditingChanged(bool value)
    {
        this.ModeTitle = value ? "Modifying a mapping" : "Creating a new mapping";
        this.SaveButtonText = value ? "Save Mapping" : "Create Mapping";

        if (value)
        {
            return;
        }

        this.TriggerControl.Clear();
        this.ActionControl.Clear();
    }

    public MappingControl()
    {
        this.InitializeComponent();

        this.Loaded += this.MappingControl_Loaded;
    }

    private void MappingControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.TriggerControl.TriggerChanged += (s, _) => this.RefreshCreateReverseMappingOption();
        this.ActionControl.ActionChanged += (s, _) => this.RefreshCreateReverseMappingOption();

        this.HoldCheckBox.Click += this.HoldCheckBox_Click;
    }

    private void HoldCheckBox_Click(object sender, RoutedEventArgs e)
        => this.dominantReverseCheckedState = this.HoldCheckBox.IsChecked == true;

    private void RefreshCreateReverseMappingOption()
    {
        if (this.TriggerControl.Trigger is IProvideReverseAspect
            && this.ActionControl.Action is IProvideReverseAspect)
        {
            this.CreateOrUpdateReverseMapping = this.dominantReverseCheckedState;
            this.HoldCheckBox.IsEnabled = true;
        }
        else
        {
            this.CreateOrUpdateReverseMapping = false;
            this.HoldCheckBox.IsEnabled = false;
        }
    }

    [RelayCommand]
    private async Task CreateMappingAsync()
    {
        var trigger = this.TriggerControl.Trigger;
        var action = this.ActionControl.Action;

        if (trigger == null)
        {
            this.ShowError("No trigger selected", "Please select a trigger before creating the mapping.");
            return;
        }

        if (action == null)
        {
            this.ShowError("No action selected", "Please select an action before creating the mapping.");
            return;
        }

        this.MappedOption ??= new MappedOption();
        this.MappedOption.Trigger = trigger;
        this.MappedOption.Action = action;

        if (!this.TriggerControl.CanMappingSave(this.MappedOption))
        {
            return;
        }

        if (!this.ActionControl.CanMappingSave(this.MappedOption))
        {
            return;
        }

        // The reverse mapping checkbox is ticked when editing a mapping with children. If the
        // user unticked it, those children should go. Only when the checkbox could be changed,
        // so children of mappings that can't be reversed are left alone.
        var childrenToRemove = new List<MappedOption>();

        if (!this.CreateOrUpdateReverseMapping
            && this.HoldCheckBox.IsEnabled
            && this.MappedOption.Children.Any())
        {
            childrenToRemove = this.MappedOption.Children.ToList();

            if (!await this.ConfirmRemoveChildMappingsAsync(childrenToRemove))
            {
                return;
            }
        }

        this.MappedOptionReverse = null;

        if (this.CreateOrUpdateReverseMapping)
        {
            var reverse = MappedOption.GenerateReverseMapping(this.MappedOption, true);

            if (!this.MappedOption.Children.Any())
            {
                this.MappedOptionReverse = reverse;
                this.MappedOptionReverse.SetParent(this.MappedOption);
            }
            else
            {
                var existingReverse = this.MappedOption.Children.First();
                existingReverse.Trigger = reverse.Trigger;
                existingReverse.Action = reverse.Action;
            }
        }

        if (childrenToRemove.Count > 0)
        {
            ChildMappingsRemoved?.Invoke(this, childrenToRemove);
        }

        MappingCreated?.Invoke(this, this.MappedOption);

        // Reset for next mapping
        this.MappedOption = null;
        this.MappedOptionReverse = null;
        this.IsEditing = false;
    }

    public void SelectMapping(MappedOption? mappedOption)
    {
        if (mappedOption == null)
        {
            this.MappedOption = null;
            this.IsEditing = false;
            return;
        }

        this.MappedOption = mappedOption;
        this.dominantReverseCheckedState = mappedOption.Children.Any();
        this.IsEditing = true;

        this.TriggerControl.SelectTrigger(mappedOption.Trigger);
        this.ActionControl.SelectAction(mappedOption.Action);
    }

    [RelayCommand]
    private void DeleteMapping()
    {
        var toDelete = this.MappedOption;

        if (toDelete == null)
        {
            return;
        }

        this.MappedOption = null;
        this.MappedOptionReverse = null;
        this.IsEditing = false;
        MappingDeleted?.Invoke(this, toDelete);
    }

    private async Task<bool> ConfirmRemoveChildMappingsAsync(IReadOnlyList<MappedOption> children)
    {
        var list = string.Join(
            Environment.NewLine,
            children.Select(child => $"• {child.Trigger} → {child.Action}")
        );

        var dialog = new ContentDialog
        {
            Title = children.Count == 1 ? "Remove child mapping?" : $"Remove {children.Count} child mappings?",
            Content = new TextBlock
            {
                Text = "You unticked creating a reverse mapping. Saving will remove "
                    + (children.Count == 1 ? "this child mapping:" : "these child mappings:")
                    + Environment.NewLine + Environment.NewLine + list,
                TextWrapping = TextWrapping.Wrap,
            },
            PrimaryButtonText = "Remove and save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot,
        };

        return await dialog.ShowAsync() == ContentDialogResult.Primary;
    }

    private void ShowError(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
        };

        _ = dialog.ShowAsync();
    }
}
