using System;
using System.Collections.Generic;
using System.Linq;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Mapping;
using Key2Joy.Mapping.Triggers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DependencyPropertyGenerator;

namespace Key2Joy.App.UserControls.Triggers;

[DependencyProperty<IEnumerable<TriggerComboBoxItem>>("TriggersAvailable")]
[DependencyProperty<TriggerComboBoxItem>("TriggerSelected")]
[DependencyProperty<object>("TriggerContent")]
public sealed partial class TriggerControl : UserControl
{
    public bool? IsTopLevel { get; set; }

    public ITriggerOptionsControl? Options { get; private set; }
    public AbstractTrigger? Trigger { get; private set; }

    public event EventHandler<TriggerChangedEventArgs>? TriggerChanged;

    private AbstractTrigger? pendingSelectTrigger;

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
        var triggerTypeFactories = TriggersRepository.GetAllTriggers(this.IsTopLevel ?? false);

        this.TriggersAvailable = triggerTypeFactories
            .Select(static kvp =>
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
            .Where(static acbi => acbi.MappingControlFactory != null)
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

        if (
            attribute != null
            && typeFactory != null
            &&
            (
                this.Trigger == null
                || this.Trigger.GetType().FullName != typeFactory.FullTypeName
            )
        )
        {
            this.Trigger = typeFactory.CreateInstance([attribute.NameFormat]);
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

        var match = trigger != null
            ? this.TriggersAvailable
                .FirstOrDefault(t => t.TypeFactory?.FullTypeName == trigger.GetType().FullName)
            : null;


        if (match == null)
        {
            return;
        }

        this.TriggerSelected = match;
        this.Options?.Select(trigger);
    }

    public bool CanMappingSave(AbstractMappedOption mappedOption)
        => this.Options?.CanMappingSave(mappedOption.Trigger) ?? false;

    partial void OnTriggerSelectedChanged(TriggerComboBoxItem? newValue)
    {
        // Unsubscribe from old options
        if (this.Options != null)
        {
            this.Options.OptionsChanged -= this.OnOptionsChanged;
        }

        if (newValue == null)
        {
            this.Options = null;
            this.TriggerContent = new Grid();
            this.BuildTrigger();
            return;
        }

        var newOptions = newValue.MappingControlFactory?.CreateInstance<ITriggerOptionsControl>();
        this.Options = newOptions;
        this.TriggerContent = newOptions;

        newOptions?.OptionsChanged += this.OnOptionsChanged;

        this.BuildTrigger();
    }

    public void Clear()
    {
        this.Trigger = null;
        this.TriggerSelected = null;
    }

    private void OnOptionsChanged(object? sender, EventArgs e) => this.BuildTrigger();
}
