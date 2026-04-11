using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Key2Joy.App.UserControls.Actions;
using Key2Joy.App.UserControls.Triggers;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Triggers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls;

[ObservableObject]
public sealed partial class MappingControl : UserControl
{
    [ObservableProperty]
    public partial bool CreateOrUpdateReverseMapping { get; set; }

    [ObservableProperty]
    public partial TriggerComboBoxItem SelectedTrigger { get; set; }

    [ObservableProperty]
    public partial ActionComboBoxItem SelectedAction { get; set; }

    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    [ObservableProperty]
    public partial string ModeTitle { get; set; } = "Creating a new mapping";

    [ObservableProperty]
    public partial string SaveButtonText { get; set; } = "Create Mapping";

    public MappedOption MappedOption { get; private set; } = null;
    public MappedOption MappedOptionReverse { get; private set; } = null;

    public event EventHandler<MappedOption> MappingCreated;
    public event EventHandler<MappedOption> MappingDeleted;
    public event EventHandler<MappedOption> MappingDeselected;

    private bool dominantReverseCheckedState;

    partial void OnIsEditingChanged(bool value)
    {
        this.ModeTitle = value ? "Modifying a mapping" : "Creating a new mapping";
        this.SaveButtonText = value ? "Save Mapping" : "Create Mapping";
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
    private void CreateMapping()
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

        MappingCreated?.Invoke(this, this.MappedOption);

        // Reset for next mapping
        this.MappedOption = null;
        this.MappedOptionReverse = null;
        this.IsEditing = false;
    }

    public void SelectMapping(MappedOption mappedOption)
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
        this.MappedOption = null;
        this.MappedOptionReverse = null;
        this.IsEditing = false;
        MappingDeleted?.Invoke(this, toDelete);
    }

    [RelayCommand]
    private void DeselectMapping()
    {
        var toDeselect = this.MappedOption;
        this.MappedOption = null;
        this.MappedOptionReverse = null;
        this.IsEditing = false;
        MappingDeselected?.Invoke(this, toDeselect);
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
