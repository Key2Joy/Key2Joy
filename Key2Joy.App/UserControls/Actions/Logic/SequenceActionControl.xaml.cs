using System;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping.Actions.Logic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

[MappingControl(
    ForType = typeof(SequenceAction),
    TextGlyph = "\uF2C7"
)]
public sealed partial class SequenceActionControl : UserControl, IActionOptionsControl
{
    public event EventHandler? OptionsChanged;

    public SequenceActionControl()
        => this.InitializeComponent();

    private SequenceActionControlItem AddActionControl(AbstractAction? action = null)
    {
        var item = new SequenceActionControlItem
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        item.RequestedRemove += (s, _) =>
        {
            this.ActionsPanel.Children.Remove(s as UIElement);
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        };

        item.RequestedMoveUp += (s, _) =>
        {
            var index = this.ActionsPanel.Children.IndexOf(s as UIElement);
            if (index > 0)
            {
                this.ActionsPanel.Children.RemoveAt(index);
                this.ActionsPanel.Children.Insert(index - 1, s as UIElement);
                OptionsChanged?.Invoke(this, EventArgs.Empty);
            }
        };

        item.RequestedMoveDown += (s, _) =>
        {
            var index = this.ActionsPanel.Children.IndexOf(s as UIElement);
            if (index < this.ActionsPanel.Children.Count - 1)
            {
                this.ActionsPanel.Children.RemoveAt(index);
                this.ActionsPanel.Children.Insert(index + 1, s as UIElement);
                OptionsChanged?.Invoke(this, EventArgs.Empty);
            }
        };

        item.ActionChanged += (s, _) =>
            OptionsChanged?.Invoke(this, EventArgs.Empty);

        this.ActionsPanel.Children.Add(item);

        if (action != null)
        {
            item.SetAction(action);
        }

        return item;
    }

    void IActionOptionsControl.Select(AbstractAction action)
    {
        var thisAction = (SequenceAction)action;

        this.ActionsPanel.Children.Clear();

        if (thisAction.ChildActions == null)
        {
            return;
        }

        foreach (var childAction in thisAction.ChildActions)
        {
            this.AddActionControl(childAction);
        }
    }

    void IActionOptionsControl.Setup(AbstractAction action)
    {
        var thisAction = (SequenceAction)action;

        thisAction.ChildActions.Clear();

        foreach (var child in this.ActionsPanel.Children)
        {
            if (child is SequenceActionControlItem item && item.Action != null)
            {
                thisAction.ChildActions.Add(item.Action);
            }
        }
    }

    bool IActionOptionsControl.CanMappingSave(AbstractAction action) => true;

    private void AddActionButton_Click(object sender, RoutedEventArgs e)
        => this.AddActionControl();
}
