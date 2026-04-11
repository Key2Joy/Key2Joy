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
            typeof(MappingControl),
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
            typeof(MappingControl),
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
            typeof(MappingControl),
            new PropertyMetadata(null));

    public object ActionContent
    {
        get => this.GetValue(ActionContentProperty);
        set => this.SetValue(ActionContentProperty, value);
    }

    public bool IsTopLevel { get; set; }

    public ActionControl()
    {
        this.InitializeComponent();

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
    }

    private static void OnActionSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // When the selected action changes, we want to update the ActionContent to be the corresponding mapping control for the selected action
        var control = (ActionControl)d;
        var selectedAction = (ActionComboBoxItem)e.NewValue;

        // If no action is selected, clear the content
        if (selectedAction == null)
        {
            control.ActionContent = new Grid();
            return;
        }

        var mappingControlFactory = selectedAction.MappingControlFactory;
        control.ActionContent = mappingControlFactory.CreateInstance<FrameworkElement>();
    }
}

public class ActionComboBoxItem
{
    public ActionAttribute ActionAttribute { get; set; }
    public MappingTypeFactory<AbstractAction> TypeFactory { get; set; }
    public MappingControlFactory MappingControlFactory { get; set; }
    public string Description { get; set; }
    public Uri ImageUri { get; set; }
}
