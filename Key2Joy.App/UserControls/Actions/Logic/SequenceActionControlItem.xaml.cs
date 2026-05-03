using System;
using Key2Joy.Contracts.Mapping.Actions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions.Logic;

public sealed partial class SequenceActionControlItem : UserControl
{
    public event EventHandler? RequestedRemove;
    public event EventHandler? RequestedMoveUp;
    public event EventHandler? RequestedMoveDown;
    public event EventHandler? ActionChanged;

    public AbstractAction? Action => this.actionControl.Action;

    public SequenceActionControlItem()
    {
        this.InitializeComponent();
        this.actionControl.IsTopLevel = false;
        this.actionControl.ActionChanged += this.ActionControl_ActionChanged;
    }

    public void SetAction(AbstractAction action)
    {
        this.Loaded += OnLoadedSelectAction;

        void OnLoadedSelectAction(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoadedSelectAction;
            this.actionControl.SelectAction(action);
        }
    }

    private void ActionControl_ActionChanged(object? sender, ActionChangedEventArgs e)
        => ActionChanged?.Invoke(this, EventArgs.Empty);

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
        => RequestedRemove?.Invoke(this, EventArgs.Empty);

    private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        => RequestedMoveUp?.Invoke(this, EventArgs.Empty);

    private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        => RequestedMoveDown?.Invoke(this, EventArgs.Empty);
}
