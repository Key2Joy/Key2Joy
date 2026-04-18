using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Key2Joy.App.Pages;
using Key2Joy.Mapping;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using Windows.UI.Core;
using DependencyPropertyGenerator;

namespace Key2Joy.App.UserControls;

[DependencyProperty<ObservableCollection<MappingGroupViewModel>>("FilteredMappingGroups")]
[DependencyProperty<int>("FilteredMappedOptionsCount")]
public sealed partial class MappingGroupsListControl : UserControl
{
    public event EventHandler<MappedOption> MappingSelected;

    /// <summary>Fired when the user wants to edit or add a mapping. Argument is <c>null</c> for "add new".</summary>
    public event EventHandler<MappedOption> EditMappingRequested;

    public event EventHandler<IList<MappedOption>> GenerateReversesRequested;

    public event EventHandler<MappedOption> RemoveMappingRequested;

    public event EventHandler<MappedOption> MakeMappingParentlessRequested;

    public event EventHandler<(MappedOption Child, MappedOption NewParent)> ChooseNewParentRequested;

    private readonly List<MappedOptionViewModel> selectedItems = [];

    public MappingGroupsListControl()
        => this.InitializeComponent();

    partial void OnFilteredMappingGroupsChanged(ObservableCollection<MappingGroupViewModel>? newValue)
    {
        foreach (var item in selectedItems)
        {
            item.IsSelected = false;
        }

        selectedItems.Clear();
    }

    private void SetSingleSelection(MappedOptionViewModel vm)
    {
        foreach (var item in this.selectedItems)
        {
            item.IsSelected = false;
        }

        this.selectedItems.Clear();

        if (vm != null)
        {
            vm.IsSelected = true;
            this.selectedItems.Add(vm);
        }
    }

    private void ToggleSelection(MappedOptionViewModel vm)
    {
        if (this.selectedItems.Contains(vm))
        {
            vm.IsSelected = false;
            this.selectedItems.Remove(vm);
        }
        else
        {
            vm.IsSelected = true;
            this.selectedItems.Add(vm);
        }
    }

    public void ExpandAll()
        => this.SetAllMappingsExpanded(true);

    public void CollapseAll()
        => this.SetAllMappingsExpanded(false);

    public void ClearSelection()
        => this.SetSingleSelection(null);

    private void MappingItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        var element = sender as FrameworkElement;
        var vm = element?.Tag as MappedOptionViewModel;

        // If right-clicking on an item that isn't part of the current selection, replace selection.
        if (vm != null && !this.selectedItems.Contains(vm))
        {
            this.SetSingleSelection(vm);
        }

        var contextItems = this.selectedItems.Count > 0
            ? this.selectedItems.ToList()
            : (
                vm != null
                ? [vm]
                : []
              );

        var builder = new MappingContextMenuBuilder(contextItems);
        builder.SelectEditMapping += (s, option) => this.EditMappingRequested?.Invoke(this, option);
        builder.SelectGenerateReverseMappings += (s, _) =>
            this.GenerateReversesRequested?.Invoke(this, contextItems.Select(x => x.Option).ToList());
        builder.SelectRemoveMappings += (s, _) => this.RemoveMappingRequested?.Invoke(this, vm?.Option);
        builder.SelectMakeMappingParentless += (s, option) => this.MakeMappingParentlessRequested?.Invoke(this, option);
        builder.SelectChooseNewParent += (s, args) => this.ChooseNewParentRequested?.Invoke(this, args);

        var flyout = builder.Build();
        flyout.ShowAt(element, new FlyoutShowOptions { Position = e.GetPosition(element) });

        e.Handled = true;
    }

    private void MappingItem_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var element = sender as FrameworkElement;
        var vm = element?.Tag as MappedOptionViewModel;

        var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
        var isCtrlHeld = (ctrlState & CoreVirtualKeyStates.Down) != 0;

        if (isCtrlHeld && vm != null)
        {
            this.ToggleSelection(vm);
        }
        else
        {
            this.SetSingleSelection(vm);
            this.MappingSelected?.Invoke(this, vm?.Option);
        }
    }

    private void MappingItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var element = sender as FrameworkElement;
        var vm = element?.Tag as MappedOptionViewModel;

        this.EditMappingRequested?.Invoke(this, vm?.Option);
        e.Handled = true;
    }

    private void ExpandToggle_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggleButton)
        {
            return;
        }

        // Find the children ItemsControl, it's the second child of the parent StackPanel
        var parentRow = toggleButton.Parent as FrameworkElement;
        var stackPanel = parentRow?.Parent as StackPanel;

        if (stackPanel == null)
        {
            return;
        }

        var childrenPanel = stackPanel.Children[1] as ItemsControl;

        if (childrenPanel == null)
        {
            return;
        }

        SetExpanded(toggleButton, childrenPanel, toggleButton.IsChecked == true);
    }

    private void SetAllMappingsExpanded(bool expanded)
    {
        var groupPanel = this.MappingGroupsControl.ItemsPanelRoot;

        if (groupPanel == null)
        {
            return;
        }

        foreach (var groupChild in groupPanel.Children)
        {
            var groupStackPanel = FindFirstChild<StackPanel>(groupChild);

            if (groupStackPanel == null || groupStackPanel.Children.Count < 2)
            {
                continue;
            }

            var itemsControl = groupStackPanel.Children[1] as ItemsControl;
            var itemsPanel = itemsControl?.ItemsPanelRoot;

            if (itemsPanel == null)
            {
                continue;
            }

            foreach (var itemChild in itemsPanel.Children)
            {
                var itemStackPanel = FindFirstChild<StackPanel>(itemChild);

                if (itemStackPanel == null || itemStackPanel.Children.Count < 2)
                {
                    continue;
                }

                var parentRow = itemStackPanel.Children[0] as Grid;
                var childrenPanel = itemStackPanel.Children[1] as ItemsControl;

                if (parentRow == null || childrenPanel == null)
                {
                    continue;
                }

                ToggleButton? toggleButton = null;

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentRow); i++)
                {
                    if (VisualTreeHelper.GetChild(parentRow, i) is ToggleButton tb)
                    {
                        toggleButton = tb;
                        break;
                    }
                }

                if (toggleButton == null || toggleButton.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                SetExpanded(toggleButton, childrenPanel, expanded);
            }
        }
    }

    private static T? FindFirstChild<T>(DependencyObject parent) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T result)
            {
                return result;
            }

            var found = FindFirstChild<T>(child);

            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static void SetExpanded(ToggleButton toggleButton, ItemsControl childrenPanel, bool expanded)
    {
        toggleButton.IsChecked = expanded;
        childrenPanel.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;

        if (toggleButton.Content is FontIcon icon)
        {
            icon.Glyph = expanded ? "\xE96D" : "\xE96E";
        }
    }
}
