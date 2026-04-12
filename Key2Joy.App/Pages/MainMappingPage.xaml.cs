using System;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions.Logic;
using Key2Joy.Mapping.Triggers.Mouse;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

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

    private void MappingItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        var element = sender as FrameworkElement;
        var vm = element?.Tag as MappedOptionViewModel;
        this.ViewModel.SelectMapping(vm?.Option);
        this.MappingControl.SelectMapping(vm?.Option);
    }

    private void MappingItem_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        var element = sender as FrameworkElement;
        var stackPanel = element?.Parent as StackPanel;

        if (stackPanel == null || stackPanel.Children.Count < 2)
        {
            return;
        }

        var childrenPanel = stackPanel.Children[1] as ItemsControl;

        if (childrenPanel == null)
        {
            return;
        }

        ToggleButton? toggleButton = null;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            if (VisualTreeHelper.GetChild(element, i) is ToggleButton tb)
            {
                toggleButton = tb;
                break;
            }
        }

        if (toggleButton == null || toggleButton.Visibility == Visibility.Collapsed)
        {
            return;
        }

        SetExpanded(toggleButton, childrenPanel, !toggleButton.IsChecked.Value);
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

    public void ExpandAllMappings() => this.SetAllMappingsExpanded(true);

    public void CollapseAllMappings() => this.SetAllMappingsExpanded(false);

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

        for (int i = 0; i < count; i++)
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

    public void SetSelectedProfile(MappingProfile profile)
        => this.ViewModel.SetSelectedProfile(profile);

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
