using System;
using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.WinUI.Controls;
using Key2Joy.Config;
using Key2Joy.Util;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Key2Joy.App.Pages;

public sealed partial class ConfigPage : Page
{
    private readonly ConfigState configState;
    private readonly List<(PropertyInfo Property, ConfigControlAttribute Attribute, FrameworkElement Control)> configControls = new();

    public ConfigPage()
    {
        this.configState = ServiceContainer.Get<IConfigManager>().GetConfigState();
        this.InitializeComponent();
        this.Loaded += this.ConfigPage_Loaded;
    }

    private void ConfigPage_Loaded(object sender, RoutedEventArgs e)
    {
        var configs = ConfigControlAttribute.GetAllProperties(typeof(ConfigState), new AttributeProvider());

        foreach (var kvp in configs)
        {
            var property = kvp.Key;
            var attribute = kvp.Value;
            var value = property.GetValue(this.configState);

            var (settingsCard, control) = this.MakeSettingsCard(attribute, value);

            if (!string.IsNullOrWhiteSpace(attribute.Hint))
            {
                settingsCard.Description = attribute.Hint;
            }

            this.pnlConfigurations.Children.Add(settingsCard);
            this.configControls.Add((property, attribute, control));
        }
    }

    private (SettingsCard Card, FrameworkElement Control) MakeSettingsCard(ConfigControlAttribute attribute, object value)
    {
        var control = this.MakeControl(attribute, value);

        var card = new SettingsCard
        {
            Header = attribute.Text,
            Content = control,
        };

        return (card, control);
    }

    private FrameworkElement MakeControl(ConfigControlAttribute attribute, object value)
    {
        switch (attribute)
        {
            case BooleanConfigControlAttribute:
            {
                var toggle = new ToggleSwitch
                {
                    IsOn = (bool)value,
                };
                toggle.Toggled += this.OnControlValueChanged;
                return toggle;
            }

            case NumericConfigControlAttribute numericAttr:
            {
                var numberBox = new NumberBox
                {
                    Minimum = numericAttr.Minimum,
                    Maximum = numericAttr.Maximum,
                    Value = Convert.ToDouble(value),
                    SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact,
                };
                numberBox.ValueChanged += this.OnControlValueChanged;
                return numberBox;
            }

            case TextConfigControlAttribute textAttr:
            {
                var textBox = new TextBox
                {
                    Text = value?.ToString() ?? string.Empty,
                    MaxLength = textAttr.MaxLength,
                };
                textBox.TextChanged += this.OnControlValueChanged;
                return textBox;
            }

            case EnumConfigControlAttribute enumAttr:
            {
                var enumValues = Enum.GetValues(enumAttr.EnumType);
                var selected = Enum.Parse(enumAttr.EnumType, value.ToString());
                var comboBox = new ComboBox
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                foreach (var enumValue in enumValues)
                {
                    comboBox.Items.Add(enumValue);
                }

                comboBox.SelectedIndex = Array.IndexOf(enumValues, selected);
                comboBox.SelectionChanged += this.OnControlValueChanged;
                return comboBox;
            }
        }

        throw new NotImplementedException("ConfigControlAttribute type not implemented: " + attribute.GetType().Name);
    }

    private void OnControlValueChanged(object sender, object e)
    {
        this.SaveAll();
    }

    private void SaveAll()
    {
        foreach (var (property, attribute, control) in this.configControls)
        {
            var value = this.GetControlValue(attribute, control);
            value = value == null
                ? value
                : Convert.ChangeType(value, property.PropertyType);
            property.SetValue(this.configState, value);
        }
    }

    private object GetControlValue(ConfigControlAttribute attribute, FrameworkElement control)
    {
        switch (attribute)
        {
            case BooleanConfigControlAttribute:
                return ((ToggleSwitch)control).IsOn;

            case NumericConfigControlAttribute:
                return ((NumberBox)control).Value;

            case TextConfigControlAttribute:
                return ((TextBox)control).Text;

            case EnumConfigControlAttribute:
                return ((ComboBox)control).SelectedItem;
        }

        throw new NotImplementedException("ConfigControlAttribute type not implemented: " + attribute.GetType().Name);
    }
}
