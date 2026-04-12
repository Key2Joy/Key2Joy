using System;
using System.Collections.Generic;
using System.Linq;
using Key2Joy.App.Pages;
using Key2Joy.Mapping;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls;

internal class MappingContextMenuBuilder
{
    /// <summary>
    /// Fired when the user wants to edit or create a mapping.
    /// The argument is the <see cref="MappedOption"/> to edit, or <c>null</c> to create a new one.
    /// </summary>
    public event EventHandler<MappedOption> SelectEditMapping;

    public event EventHandler SelectGenerateReverseMappings;

    public event EventHandler SelectRemoveMappings;

    public event EventHandler<MappedOption> SelectMakeMappingParentless;

    public event EventHandler<(MappedOption Child, MappedOption NewParent)> SelectChooseNewParent;

    private readonly IList<MappedOptionViewModel> selectedItems;

    // Tracks the mapping that is waiting for the user to pick its new parent.
    private static MappedOption currentChildChoosingParent;

    internal MappingContextMenuBuilder(IList<MappedOptionViewModel> selectedItems)
        => this.selectedItems = selectedItems;

    private static MenuFlyoutItem CreateItem(string text, string glyph = null)
    {
        var item = new MenuFlyoutItem { Text = text };

        if (glyph != null)
        {
            item.Icon = new FontIcon { Glyph = glyph };
        }

        return item;
    }

    private void SetupMultiSelectionItems(MenuFlyout flyout)
    {
        var count = this.selectedItems.Count;

        // Multi-edit property dialog not yet available in WinUI3.
        var editItem = CreateItem("Edit Multiple Mappings");
        editItem.IsEnabled = false;
        flyout.Items.Add(editItem);

        flyout.Items.Add(new MenuFlyoutSeparator());

        var removeItem = CreateItem($"Remove {count} Mappings", "\uE74D");
        removeItem.Click += (s, _) => this.SelectRemoveMappings?.Invoke(this, EventArgs.Empty);
        flyout.Items.Add(removeItem);
    }

    internal MenuFlyout Build()
    {
        var flyout = new MenuFlyout();

        var addItem = CreateItem("Add New Mapping", "\uE710");
        addItem.Click += (s, _) => this.SelectEditMapping?.Invoke(this, null);
        flyout.Items.Add(addItem);

        var generateItem = CreateItem("Generate Reverse Mappings", "\uE895");
        generateItem.Click += (s, _) => this.SelectGenerateReverseMappings?.Invoke(this, EventArgs.Empty);
        flyout.Items.Add(generateItem);

        var count = this.selectedItems.Count;

        if (count == 0)
        {
            return flyout;
        }

        flyout.Items.Add(new MenuFlyoutSeparator());

        if (count > 1)
        {
            this.SetupMultiSelectionItems(flyout);
            return flyout;
        }

        var mappedOption = this.selectedItems[0].Option;

        var removeItem = CreateItem("Remove Mapping", "\uE74D");
        removeItem.Click += (s, _) => this.SelectRemoveMappings?.Invoke(this, EventArgs.Empty);
        flyout.Items.Add(removeItem);

        flyout.Items.Add(new MenuFlyoutSeparator());

        if (mappedOption.IsChild)
        {
            var detachItem = CreateItem("Disconnect Mapping from Parent");
            detachItem.Click += (s, _) => this.SelectMakeMappingParentless?.Invoke(this, mappedOption);
            flyout.Items.Add(detachItem);
        }

        if (currentChildChoosingParent == null)
        {
            var chooseParentItem = CreateItem("Choose New Parent for this Mapping...");
            chooseParentItem.Click += (s, _) => currentChildChoosingParent = mappedOption;
            chooseParentItem.IsEnabled = !mappedOption.Children.Any();
            flyout.Items.Add(chooseParentItem);
        }
        else
        {
            var assignParentItem = CreateItem("Choose as Parent", "\uE73E");
            assignParentItem.Click += (s, _) =>
            {
                this.SelectChooseNewParent?.Invoke(this, (currentChildChoosingParent, mappedOption));
                currentChildChoosingParent = null;
            };
            assignParentItem.IsEnabled = !mappedOption.IsChild;
            flyout.Items.Add(assignParentItem);

            var cancelItem = CreateItem("Cancel Choosing Parent", "\uE711");
            cancelItem.Click += (s, _) => currentChildChoosingParent = null;
            flyout.Items.Add(cancelItem);
        }

        return flyout;
    }
}
