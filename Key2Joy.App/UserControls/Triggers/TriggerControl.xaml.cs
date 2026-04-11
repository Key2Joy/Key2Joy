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
            typeof(TriggerControl),
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
            typeof(TriggerControl),
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
            typeof(TriggerControl),
            new PropertyMetadata(null));

    public object TriggerContent
    {
        get => this.GetValue(TriggerContentProperty);
        set => this.SetValue(TriggerContentProperty, value);
    }

    public bool IsTopLevel { get; set; }

    public ITriggerOptionsControl Options { get; private set; }
    public AbstractTrigger Trigger { get; private set; }

    public event EventHandler<TriggerChangedEventArgs> TriggerChanged;

    private AbstractTrigger pendingSelectTrigger;

    public TriggerControl()
    {
        this.InitializeComponent();

        this.Loaded += this.TriggerControl_Loaded;
    }

    private void TriggerControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.TriggerControl_Loaded;
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

        if (this.pendingSelectTrigger != null)
        {
            this.SelectTrigger(this.pendingSelectTrigger);
            this.pendingSelectTrigger = null;
        }
    }

    private void BuildTrigger()
    {
        if (this.TriggerSelected == null)
        {
            TriggerChanged?.Invoke(this, TriggerChangedEventArgs.Empty);
            return;
        }

        var attribute = this.TriggerSelected.TriggerAttribute;
        var typeFactory = this.TriggerSelected.TypeFactory;

        if (this.Trigger == null || this.Trigger.GetType().FullName != typeFactory.FullTypeName)
        {
            this.Trigger = typeFactory.CreateInstance(new object[] { attribute.NameFormat });
        }

        this.Options?.Setup(this.Trigger);

        TriggerChanged?.Invoke(this, new TriggerChangedEventArgs(this.Trigger));
    }

    public void SelectTrigger(AbstractTrigger trigger)
    {
        if (this.TriggersAvailable == null)
        {
            this.pendingSelectTrigger = trigger;
            return;
        }

        var match = this.TriggersAvailable
            .FirstOrDefault(t => t.TypeFactory.FullTypeName == trigger.GetType().FullName);

        if (match == null)
        {
            return;
        }

        this.TriggerSelected = match;
        this.Options?.Select(trigger);
    }

    public bool CanMappingSave(AbstractMappedOption mappedOption)
        => this.Options?.CanMappingSave(mappedOption.Trigger) ?? false;

    private static void OnTriggerSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TriggerControl)d;
        var selectedTrigger = (TriggerComboBoxItem)e.NewValue;

        // Unsubscribe from old options
        if (control.Options != null)
        {
            control.Options.OptionsChanged -= control.OnOptionsChanged;
        }

        if (selectedTrigger == null)
        {
            control.Options = null;
            control.TriggerContent = new Grid();
            control.BuildTrigger();
            return;
        }

        var newOptions = selectedTrigger.MappingControlFactory.CreateInstance<ITriggerOptionsControl>();
        control.Options = newOptions;
        control.TriggerContent = newOptions;

        newOptions.OptionsChanged += control.OnOptionsChanged;

        control.BuildTrigger();
    }

    private void OnOptionsChanged(object sender, EventArgs e) => this.BuildTrigger();
}
