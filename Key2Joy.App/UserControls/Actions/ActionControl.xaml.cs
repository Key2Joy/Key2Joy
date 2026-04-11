using System;
using System.Collections.Generic;
using System.Linq;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Actions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DependencyPropertyGenerator;

namespace Key2Joy.App.UserControls.Actions;

[DependencyProperty<IEnumerable<ActionComboBoxItem>>("ActionsAvailable")]
[DependencyProperty<ActionComboBoxItem>("ActionSelected")]
[DependencyProperty<object>("ActionContent")]
public sealed partial class ActionControl : UserControl
{
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

    partial void OnActionSelectedChanged(ActionComboBoxItem selectedAction)
    {
        // Unsubscribe from old options
        if (this.Options != null)
        {
            this.Options.OptionsChanged -= this.OnOptionsChanged;
        }

        if (selectedAction == null)
        {
            this.Options = null;
            this.Action = null;
            this.ActionContent = new Grid();
            this.BuildAction();
            return;
        }

        var newOptions = selectedAction.MappingControlFactory.CreateInstance<IActionOptionsControl>();
        this.Options = newOptions;
        this.ActionContent = newOptions;

        newOptions.OptionsChanged += this.OnOptionsChanged;

        this.BuildAction();
    }

    private void OnOptionsChanged(object sender, EventArgs e) => this.BuildAction();
}
