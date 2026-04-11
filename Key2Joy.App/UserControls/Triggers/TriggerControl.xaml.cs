using System;
using System.Collections.Generic;
using System.Linq;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Triggers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.UserControls.Triggers;

public sealed partial class TriggerControl : UserControl
{
    public static readonly DependencyProperty TriggersAvailableProperty =
        DependencyProperty.Register(
            nameof(TriggersAvailable),
            typeof(IEnumerable<TriggerComboBoxItem>),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public IEnumerable<TriggerComboBoxItem> TriggersAvailable
    {
        get => (IEnumerable<TriggerComboBoxItem>)this.GetValue(TriggersAvailableProperty);
        set => this.SetValue(TriggersAvailableProperty, value);
    }

    public static readonly DependencyProperty TriggerSelectedProperty =
        DependencyProperty.Register(
            nameof(TriggerSelected),
            typeof(TriggerComboBoxItem),
            typeof(MappingControl),
            new PropertyMetadata(null, OnTriggerSelected));

    public TriggerComboBoxItem TriggerSelected
    {
        get => (TriggerComboBoxItem)this.GetValue(TriggerSelectedProperty);
        set => this.SetValue(TriggerSelectedProperty, value);
    }

    public static readonly DependencyProperty TriggerContentProperty =
        DependencyProperty.Register(
            nameof(TriggerContent),
            typeof(object),
            typeof(MappingControl),
            new PropertyMetadata(null));

    public object TriggerContent
    {
        get => this.GetValue(TriggerContentProperty);
        set => this.SetValue(TriggerContentProperty, value);
    }

    public bool IsTopLevel { get; set; }

    public TriggerControl()
    {
        this.InitializeComponent();

        this.LoadTriggers();
    }

    private void LoadTriggers()
    {
        var triggerTypeFactories = TriggersRepository.GetAllTriggers(this.IsTopLevel);

        this.TriggersAvailable = triggerTypeFactories
            .Select(kvp =>
            {
                var mappingControlFactory = MappingControlRepository.GetMappingControlFactory(kvp.Value.FullTypeName);
                var customImage = mappingControlFactory?.ImageResourceName;

                return new TriggerComboBoxItem
                {
                    TriggerAttribute = kvp.Key,
                    TypeFactory = kvp.Value,
                    Description = kvp.Key.Description,
                    MappingControlFactory = mappingControlFactory,
                    ImageUri = new Uri(customImage ?? "ms-appx:///Assets/StoreLogo.png")
                };
            })
            .Where(acbi => acbi.MappingControlFactory != null)
            .ToList();
    }

    private static void OnTriggerSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // When the selected trigger changes, we want to update the TriggerContent to be the corresponding mapping control for the selected trigger
        var control = (TriggerControl)d;
        var selectedTrigger = (TriggerComboBoxItem)e.NewValue;

        // If no trigger is selected, clear the content
        if (selectedTrigger == null)
        {
            control.TriggerContent = new Grid();
            return;
        }

        var mappingControlFactory = selectedTrigger.MappingControlFactory;
        control.TriggerContent = mappingControlFactory.CreateInstance<FrameworkElement>();
    }
}

public class TriggerComboBoxItem
{
    public TriggerAttribute TriggerAttribute { get; set; }
    public MappingTypeFactory<AbstractTrigger> TypeFactory { get; set; }
    public MappingControlFactory MappingControlFactory { get; set; }
    public string Description { get; set; }
    public Uri ImageUri { get; set; }
}
