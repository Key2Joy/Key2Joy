using System;
using System.Collections.Generic;
using System.Linq;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Actions;

public sealed partial class ActionControl : UserControl
{
    public static readonly DependencyProperty ActionsAvailableProperty =
        DependencyProperty.Register(
            nameof(ActionsAvailable),
            typeof(IEnumerable<ActionComboBoxItem>),
            typeof(ActionControl),
            new PropertyMetadata(null));

    public IEnumerable<ActionComboBoxItem> ActionsAvailable
    {
        get => (IEnumerable<ActionComboBoxItem>)this.GetValue(ActionsAvailableProperty);
        set => this.SetValue(ActionsAvailableProperty, value);
    }

    public static readonly DependencyProperty ActionSelectedProperty =
        DependencyProperty.Register(
            nameof(ActionSelected),
            typeof(ActionComboBoxItem),
            typeof(ActionControl),
            new PropertyMetadata(null, OnActionSelected));

    public ActionComboBoxItem ActionSelected
    {
        get => (ActionComboBoxItem)this.GetValue(ActionSelectedProperty);
        set => this.SetValue(ActionSelectedProperty, value);
    }

    public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(
            nameof(ActionContent),
            typeof(object),
            typeof(ActionControl),
            new PropertyMetadata(null));

    public object ActionContent
    {
        get => this.GetValue(ActionContentProperty);
        set => this.SetValue(ActionContentProperty, value);
    }

    public bool IsTopLevel { get; set; }

    public IActionOptionsControl Options { get; private set; }
    public AbstractAction Action { get; private set; }

    public event EventHandler<ActionChangedEventArgs> ActionChanged;

    private AbstractAction pendingSelectAction;

    public ActionControl()
    {
        this.InitializeComponent();

        this.Loaded += this.ActionControl_Loaded;
    }

    private void ActionControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.ActionControl_Loaded;
        this.LoadActions();
    }

    private void LoadActions()
    {
        var actionTypeFactories = ActionsRepository.GetAllActions(this.IsTopLevel);

        this.ActionsAvailable = actionTypeFactories
            .Select(kvp =>
            {
                var mappingControlFactory = MappingControlRepository.GetMappingControlFactory(kvp.Value.FullTypeName);
                var customImage = mappingControlFactory?.ImageResourceName;

                return new ActionComboBoxItem
                {
                    ActionAttribute = kvp.Key,
                    TypeFactory = kvp.Value,
                    Description = kvp.Key.Description,
                    MappingControlFactory = mappingControlFactory,
                    ImageUri = new Uri(customImage ?? "ms-appx:///Assets/StoreLogo.png")
                };
            })
            .Where(acbi => acbi.MappingControlFactory != null)
            .ToList();

        if (this.pendingSelectAction != null)
        {
            this.SelectAction(this.pendingSelectAction);
            this.pendingSelectAction = null;
        }
    }

    private void BuildAction()
    {
        if (this.ActionSelected == null)
        {
            ActionChanged?.Invoke(this, new ActionChangedEventArgs(null));
            return;
        }

        var typeFactory = this.ActionSelected.TypeFactory;

        if (this.Action == null || this.Action.GetType().FullName != typeFactory.FullTypeName)
        {
            this.Action = CoreAction.MakeAction(typeFactory);
        }

        this.Options?.Setup(this.Action);

        ActionChanged?.Invoke(this, new ActionChangedEventArgs(this.Action));
    }

    public void SelectAction(AbstractAction action)
    {
        if (this.ActionsAvailable == null)
        {
            this.pendingSelectAction = action;
            return;
        }

        var actionFullTypeName = MappingTypeHelper.GetTypeFullName(ActionsRepository.GetAllActions(), action);
        actionFullTypeName = MappingTypeHelper.EnsureSimpleTypeName(actionFullTypeName);

        var match = this.ActionsAvailable
            .FirstOrDefault(a => a.TypeFactory.FullTypeName == actionFullTypeName);

        if (match == null)
        {
            return;
        }

        this.ActionSelected = match;
        this.Options?.Select(action);
    }

    public bool CanMappingSave(AbstractMappedOption mappedOption)
        => this.Options?.CanMappingSave(mappedOption.Action) ?? false;

    private static void OnActionSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ActionControl)d;
        var selectedAction = (ActionComboBoxItem)e.NewValue;

        // Unsubscribe from old options
        if (control.Options != null)
        {
            control.Options.OptionsChanged -= control.OnOptionsChanged;
        }

        if (selectedAction == null)
        {
            control.Options = null;
            control.Action = null;
            control.ActionContent = new Grid();
            control.BuildAction();
            return;
        }

        var newOptions = selectedAction.MappingControlFactory.CreateInstance<IActionOptionsControl>();
        control.Options = newOptions;
        control.ActionContent = newOptions;

        newOptions.OptionsChanged += control.OnOptionsChanged;

        control.BuildAction();
    }

    private void OnOptionsChanged(object sender, EventArgs e) => this.BuildAction();
}
